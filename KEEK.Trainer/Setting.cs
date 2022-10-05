namespace KEEK.Trainer;

public class Setting : MonoBehaviour {
    public static ConfigEntry<bool> SkipLogo;
    public static ConfigEntry<bool> RepeatLevel;
    public static ConfigEntry<bool> QuickRetryOnDeath;
    public static ConfigEntry<bool> ShowLevelNumber;
    public static ConfigEntry<bool> ClearCollectedNumber;
    public static ConfigEntry<bool> ShowLevelTimer;
    public static ConfigEntry<bool> Music;
    public static ConfigEntry<bool> Sound;
    public static ConfigEntry<bool> LoadStateAfterDeathOrFinishLevel;
    public static ConfigEntry<string> MusicSelector;
    public static ConfigEntry<KeyboardShortcut> QuickRetry;
    public static ConfigEntry<KeyboardShortcut> PreviousLevel;
    public static ConfigEntry<KeyboardShortcut> NextLevel;
    public static ConfigEntry<KeyboardShortcut> Teleport;
    public static ConfigEntry<KeyboardShortcut> SaveState;
    public static ConfigEntry<KeyboardShortcut> LoadState;
    public static ConfigEntry<KeyboardShortcut> ClearState;
    public static ConfigEntry<int> LevelSelector;

    public static event Action OnKeyUpdate;

    public void Awake() {
        ConfigFile config = Plugin.Instance.Config;

        RepeatLevel = config.Bind("Level", "Play Level On Repeat", true, -1);
        QuickRetry = config.Bind("Level", "Quick Retry", new KeyboardShortcut(KeyCode.R));
        PreviousLevel = config.Bind("Level", "Previous Level", new KeyboardShortcut(KeyCode.PageUp));
        NextLevel = config.Bind("Level", "Next Level", new KeyboardShortcut(KeyCode.PageDown));

        Music = config.Bind("Audio", "Music", true, 1);
        Sound = config.Bind("Audio", "Sound", true, 2);

        ClearCollectedNumber = config.Bind("Other", "Clear Collected Numbers And Save Point", true);
        QuickRetryOnDeath = config.Bind("Other", "Quick Retry On Death", true);
        ShowLevelNumber = config.Bind("Other", "Show Level Number", true);
        ShowLevelTimer = config.Bind("Other", "Show Level Timer", true);
        SkipLogo = config.Bind("Other", "Skip Logo", true);
        Teleport = config.Bind("Other", "Teleport", new KeyboardShortcut(KeyCode.T));

        LoadStateAfterDeathOrFinishLevel = config.Bind("State", "Load State After Death/Finish Level", true, -1);
        SaveState = config.Bind("State", "Save State", new KeyboardShortcut(KeyCode.E));
        LoadState = config.Bind("State", "Load State", new KeyboardShortcut(KeyCode.Q));
        ClearState = config.Bind("State", "Clear State", new KeyboardShortcut(KeyCode.C));
    }

    private void Update() {
        if (PauseMenu.Instance?.Menu?.activeSelf != false) {
            return;
        }

        if (!PauseMenu.Instance.EnableCalling && SceneManager.GetActiveScene().name != "LevelFinal") {
            return;
        }

        OnKeyUpdate?.Invoke();
    }
}