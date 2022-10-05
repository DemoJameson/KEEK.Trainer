namespace KEEK.Trainer.Features;

public class LevelSelector : BaseFeature {
    private static readonly List<int> LevelIndexes = new();

    private void Awake() {
        HookMethods();
        InitLevels();
        Setting.OnKeyUpdate += SettingOnKeyOnKeyUpdate;
    }

    private void SettingOnKeyOnKeyUpdate() {
        if (Setting.QuickRetry.IsMainKeyDown()) {
            ReloadLevel();
        } else if (Setting.NextLevel.IsMainKeyDown()) {
            GoToNextLevel();
        } else if (Setting.PreviousLevel.IsMainKeyDown()) {
            GoToPreviousLevel();
        }
    }

    private void HookMethods() {
        HookUtils.Hook(typeof(NextLevel), "SendGoToNextLevelMessage", SendGoToNextLevelMessage);
        HookUtils.ActiveSceneChanged(OnSceneChanged);
    }

    // only include title and levels
    private void InitLevels() {
        // 1: title
        LevelIndexes.Add(1);

        // skip 0:Team 1:Title 2:Bonus Last:Result
        for (int i = 3; i < SceneManager.sceneCountInBuildSettings - 1; i++) {
            LevelIndexes.Add(i);
        }

        int[] levelNumbers = LevelIndexes.Select((_, j) => j).ToArray();
        Setting.LevelSelector = Plugin.Instance.Config.Bind("Level", "Level Selector", 0,
            new ConfigDescription("Go to the level", new AcceptableValueList<int>(levelNumbers)));
        Setting.LevelSelector.SettingChanged += OnLevelSelectorOnSettingChanged;
    }

    private void OnSceneChanged(Scene _, Scene newScene) {
        if (LevelIndexes.IndexOf(newScene.buildIndex) != -1) {
            Setting.LevelSelector.Value = LevelIndexes.IndexOf(newScene.buildIndex);
        }

        if (LevelIndexes.IndexOf(newScene.buildIndex) == 1) {
            GlobalData.CurrentGameTime = 0;
        }
    }

    private void OnLevelSelectorOnSettingChanged(object obj, EventArgs eventArgs) {
        int index = LevelIndexes[Setting.LevelSelector.Value];
        if (index != ActiveBuildIndex) {
            SceneManager.LoadScene(index);
        }
    }

    private void SendGoToNextLevelMessage(Action<NextLevel, string> orig, NextLevel self, string levelName) {
        if (Setting.RepeatLevel.Value && ActiveName != Title) {
            ReloadLevel();
        } else {
            orig(self, levelName);
        }
    }

    private void ReloadLevel() {
        SceneManager.LoadScene(ActiveBuildIndex);
    }

    private void GoToPreviousLevel() {
        int index = LevelIndexes.IndexOf(ActiveBuildIndex);
        if (index > 0) {
            SceneManager.LoadScene(LevelIndexes[index - 1]);
        }
    }

    private void GoToNextLevel() {
        int index = LevelIndexes.IndexOf(ActiveBuildIndex);
        if (index < LevelIndexes.Count - 1) {
            SceneManager.LoadScene(LevelIndexes[index + 1]);
        }
    }
}