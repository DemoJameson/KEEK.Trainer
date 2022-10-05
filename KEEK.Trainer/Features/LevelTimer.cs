namespace KEEK.Trainer.Features;

public class LevelTimer : BaseFeature {
    private GUIStyle style;
    private GUIStyle goldStyle;
    private string level;
    private float time;
    private float lastTime;
    private float pb;
    private float lastPb;

    private void Awake() {
        style = new GUIStyle {
            alignment = TextAnchor.MiddleRight,
            fontSize = 16,
            normal = {
                textColor = Color.white
            }
        };

        goldStyle = new GUIStyle(style) {
            normal = {
                textColor = Color.yellow
            }
        };

        level = ActiveName;

        HookUtils.ActiveSceneChanged(OnSceneChanged);
        HookUtils.Hook(typeof(NextLevel), "GoToNextLevel", GoToNextLevel);
    }

    private void OnSceneChanged(Scene _, Scene newScene) {
        time = 0;
        if (level != newScene.name) {
            level = newScene.name;
            pb = 0;
            lastPb = 0;
            lastTime = 0;
        }
    }

    private void GoToNextLevel(Action<NextLevel, CharacterController2D> orig, NextLevel self, CharacterController2D character) {
        FinishLevel();
        orig(self, character);
    }

    private void FinishLevel() {
        lastTime = time;
        lastPb = pb;

        if (pb == 0 || pb > time) {
            pb = time;
        }
    }

    private void Update() {
        // final room
        if (ActiveName == "LevelFinal" && GhostBossFightManager.Instance.Paused) {
            FinishLevel();
        } else {
            time += Time.deltaTime;
        }
    }

    private void OnGUI() {
        if (!Setting.ShowLevelTimer.Value || !ActiveName.StartsWith("Level")) {
            return;
        }

        bool beatPb = lastPb > 0 && lastTime < lastPb;
        Rect rect = new(Screen.width - 105, Screen.height - 120, 80, 50);
        GUI.Label(new Rect(rect.x, rect.y - 40, 80, 50), $"PB{FormatTime(pb)}", style);
        GUI.Label(new Rect(rect.x, rect.y - 20, 80, 50),
            $"{ComparePb(lastTime, lastPb)}{FormatTime(lastTime)}",
            beatPb ? goldStyle : style);
        GUI.Label(new Rect(rect.x, rect.y, 80, 50), FormatTime(time), style);
    }

    private static string FormatTime(float time) {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return "  " + timeSpan.ToString(timeSpan.TotalSeconds < 60 ? "s\\.ff" : "m\\:ss\\.ff");
    }
    
    private static string ComparePb(float time, float pbTime) {
        if (pbTime == 0) {
            return "";
        }

        float difference = time - pbTime;

        if (difference == 0) {
            return " +0.0";
        }

        TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Abs(difference));
        string result = difference >= 0 ? " +" : " -";
        result += (int)timeSpan.TotalSeconds + timeSpan.ToString("\\.ff");
        return result;
    }
}