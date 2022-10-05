namespace KEEK.Trainer.Features;

/// <summary>
/// All features will be added in Plugin.Awake();
/// </summary>
public abstract class BaseFeature : MonoBehaviour {
    // ReSharper disable once UnusedMember.Global
    public static ManualLogSource Logger => Plugin.Log;
    public const string Title = "Title";
    public static Scene ActiveScene => SceneManager.GetActiveScene();
    public static string ActiveName => ActiveScene.name;
    public static int ActiveBuildIndex => ActiveScene.buildIndex;

    public static void Initialize(GameObject gameObject) {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
            if (type.IsSubclassOf(typeof(BaseFeature))) {
                gameObject.AddComponent(type);
            }
        }
    }
}