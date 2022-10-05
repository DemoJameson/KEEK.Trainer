namespace KEEK.Trainer.Features; 

public class TeleportToMouse : BaseFeature {
    private void Awake() {
        Setting.OnKeyUpdate += () => {
            if (Setting.Teleport.IsMainKeyDown()) {
                CharacterController2D player = GameManager.Instance.PlayerCharacter;
                Vector3 mousePos = UnityInput.Current.mousePosition;
                mousePos.z = player.transform.position.z - Camera.main.transform.position.z ;

                // fixme: can't teleport when stuck in the wall
                player.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
                
                player.SetHorizontalSpeed(0);
                player.SetVerticalSpeed(0);
            }
        };
    }
}