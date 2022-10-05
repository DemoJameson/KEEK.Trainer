namespace KEEK.Trainer.Features;

public class ClearCollection : BaseFeature {
    private void Awake() {
        HookUtils.ActiveSceneChanged(OnSceneChanged);
    }

    private void OnSceneChanged(Scene _, Scene newScene) {
        if (Setting.ClearCollectedNumber.Value) {
            GlobalData.LastSavePointName = null;
            GlobalData.ManualSaved = false;
            GlobalData.NumberWaitingGather = -1;
            GlobalData.CollectedNumbers.Clear();
            SavingUtil.GottenNumbersList?.Clear();
        }
    }
}