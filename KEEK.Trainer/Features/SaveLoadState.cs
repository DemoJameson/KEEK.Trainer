namespace KEEK.Trainer.Features;

public class SaveLoadState : BaseFeature {
    private bool IsSaved => savedBuildIndex != null;
    private int? savedBuildIndex;
    private readonly Dictionary<string, ComponentState> savedObjectStates = new();
    private Coroutine loadStateCoroutine;

    private void Awake() {
        Setting.OnKeyUpdate += () => {
            if (Setting.SaveState.IsMainKeyDown()) {
                SaveState();
            } else if (Setting.LoadState.IsMainKeyDown()) {
                LoadState();
            }
        };
        HookUtils.Hook(typeof(GameManager), "PlayerDead", PlayerDead);
        HookUtils.Hook(typeof(NextLevel), "SendGoToNextLevelMessage", SendGoToNextLevelMessage);
    }

    private void Update() {
        if (PauseMenu.Instance?.Menu?.activeSelf != false) {
            return;
        }

        if (Setting.ClearState.IsMainKeyDown()) {
            ClearState();
        } 
    }

    private void PlayerDead(Action<GameManager> orig, GameManager self) {
        if (Setting.LoadStateAfterDeathOrFinishLevel.Value && IsSaved) {
            LoadState();
        } else {
            orig(self);
        }
    }
    
    private void SendGoToNextLevelMessage(Action<NextLevel, string> orig, NextLevel self, string levelName) {
        if (IsSaved && Setting.LoadStateAfterDeathOrFinishLevel.Value && ActiveName != Title) {
            LoadState();
        } else {
            orig(self, levelName);
        }
    }

    private void SaveState() {
        ClearState(false);

        if (FindObjectOfType<CharacterController2D>() is { } player) {
            Toast.Show("Save State");
            
            savedBuildIndex = ActiveBuildIndex;
            savedObjectStates[player.name] = new ComponentState(player);
            foreach (MovingObject movingObject in FindObjectsOfType<MovingObject>()) {
                // make sure the key is unique
                savedObjectStates[movingObject.name + movingObject.Path[0].position] = new ComponentState(movingObject);
            }
        }
    }

    private void LoadState() {
        if (!IsSaved) {
            return;
        }

        Toast.Show("Load State");
        SceneManager.LoadScene(savedBuildIndex.Value);
        if (loadStateCoroutine != null) {
            StopCoroutine(loadStateCoroutine);
        }
        loadStateCoroutine = StartCoroutine(RestoreStates());
    }

    private IEnumerator RestoreStates() {
        if (IsSaved) {
            yield return new WaitForFixedUpdate();
            // anyway, add this to avoid emitting light after loading state
            yield return new WaitForFixedUpdate();

            Dictionary<MonoBehaviour, bool> savedEnabled = new();

            List<MonoBehaviour> behaviours = FindObjectsOfType<MovingObject>().Cast<MonoBehaviour>().ToList();
            if (FindObjectOfType<CharacterController2D>() is { } player) {
                behaviours.Add(player);
            }

            foreach (MonoBehaviour monoBehaviour in behaviours) {
                string key = monoBehaviour.name;
                if (monoBehaviour is MovingObject movingObject) {
                    key = movingObject.name + movingObject.Path[0].position;
                }
                if (savedObjectStates.TryGetValue(key, out var state)) {
                    state.CloneInto(monoBehaviour);
                    savedEnabled[monoBehaviour] = monoBehaviour.enabled;
                    monoBehaviour.enabled = false;
                }
            }

            if (FindObjectOfType<CharacterController2D>() is { } character) {
                character._sprite.transform.localScale = character.FacingRight ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);
            }

            // wait for screen wipe finished then unfreeze
            yield return new WaitForSeconds(0.6f);
            foreach (MonoBehaviour behaviour in savedEnabled.Keys) {
                behaviour.enabled = savedEnabled[behaviour];
            }
        }
    }

    private void ClearState(bool showToast = true) {
        if (showToast) {
            Toast.Show("Clear State");
        }

        if (loadStateCoroutine != null) {
            StopCoroutine(loadStateCoroutine);
        }

        savedBuildIndex = null;
        savedObjectStates.Clear();
    }
}

internal record ComponentState {
    private readonly Vector3 position;
    private readonly Dictionary<FieldInfo, object> values = new();

    public ComponentState(Component component) {
        position = component.transform.position;
        foreach (FieldInfo fieldInfo in component.GetType().GetAllSimpleFieldInfos()) {
            values[fieldInfo] = fieldInfo.GetValue(component);
        }
    }

    public void CloneInto(Component component) {
        component.transform.position = position;
        foreach (FieldInfo fieldInfo in values.Keys) {
            fieldInfo.SetValue(component, values[fieldInfo]);
        }
    }
}