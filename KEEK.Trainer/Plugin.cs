using KEEK.Trainer.Features;

namespace KEEK.Trainer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin {
    public static Plugin Instance { get; private set; }
    public static ManualLogSource Log => Instance.Logger;

    private void Awake() {
        Instance = this;

        // The settings needs to be initialized first
        gameObject.AddComponent<Setting>();
        BaseFeature.Initialize(gameObject);
    }

    private void OnDestroy() {
        HookUtils.Dispose();
    }

    private void OnGUI() {
        GUI.Label(new Rect(Screen.width - 105, 20, 105, 50), $"Trainer v{MyPluginInfo.PLUGIN_VERSION}");
    }
}