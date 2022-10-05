namespace KEEK.Trainer.Utils; 

public static class ConfigEntryExtensions {
    public static bool IsMainKeyDown(this ConfigEntry<KeyboardShortcut> configEntry) {
        return UnityInput.Current.GetKeyDown(configEntry.Value.MainKey);
    }
}