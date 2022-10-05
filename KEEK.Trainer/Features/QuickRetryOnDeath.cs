namespace KEEK.Trainer.Features;

public class QuickRetryOnDeath : BaseFeature {
    private void Awake() {
        HookUtils.Hook(typeof(GameManager), "PlayerDead", PlayerDead);
    }

    private void PlayerDead(Action<GameManager> orig, GameManager self) {
        if (Setting.QuickRetryOnDeath.Value) {
            SceneManager.LoadScene(ActiveBuildIndex);
        } else {
            orig(self);
        }
    }
}