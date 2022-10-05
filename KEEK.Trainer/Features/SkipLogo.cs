namespace KEEK.Trainer.Features; 

public class SkipLogo : BaseFeature {
    private void Awake() {
        if (Setting.SkipLogo.Value && ActiveName == "Team") {
            SceneManager.LoadScene("Title");
        }
    }
}