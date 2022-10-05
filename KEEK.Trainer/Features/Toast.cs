namespace KEEK.Trainer.Features; 

public class Toast : BaseFeature {
    private static Toast instance;
    private string text = "";
    
    public static void Show(string text) {
        instance.StopAllCoroutines();
        instance.text = text;
        instance.StartCoroutine(ClearToast());
    }

    private static IEnumerator ClearToast() {
        yield return new WaitForSeconds(0.6f);
        instance.text = "";
    }
    
    private void Awake() {
        if (!Equals(instance, this)) {
            instance = this;
        }
    }

    private void OnGUI() {
        if (text.Length > 0) {
            GUI.Label(new Rect(23, Screen.height - 40, 100, 50), text);
        }
    }
}