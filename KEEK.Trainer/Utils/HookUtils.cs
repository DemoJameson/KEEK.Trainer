using UnityEngine.Events;

namespace KEEK.Trainer.Utils; 

public static class HookUtils {
    private static readonly List<IDetour> Hooks = new();
    private static readonly List<UnityAction<Scene, Scene>> Actions = new();

    public static void Hook(Type type, string name, Delegate to) {
        if (type.GetMethod(name, (BindingFlags) (-1)) is {} method) {
            Hooks.Add(new Hook(method, to));
        } else {
            Plugin.Log.LogWarning($"Method {name} does not exist in {type.FullName}");
        }
    }

    public static void ActiveSceneChanged(UnityAction<Scene, Scene> action) {
        Actions.Add(action);
        SceneManager.activeSceneChanged += action;
    }

    public static void Dispose() {
        foreach (IDetour detour in Hooks) {
            detour.Dispose();
        }
        
        foreach (UnityAction<Scene,Scene> action in Actions) {
            SceneManager.activeSceneChanged -= action;
        }

        Hooks.Clear();
        Actions.Clear();
    }
}