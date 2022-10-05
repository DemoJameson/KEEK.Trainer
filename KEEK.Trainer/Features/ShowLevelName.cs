namespace KEEK.Trainer.Features;

public class ShowLevelName : BaseFeature {
    private void OnGUI() {
        if (!ActiveName.StartsWith("Level")) {
            return;
        }

        if (Setting.ShowLevelNumber.Value) {
            GUI.Label(new Rect(23, 20, 100, 100), $"Level {Setting.LevelSelector.Value}");
        }
    }
}