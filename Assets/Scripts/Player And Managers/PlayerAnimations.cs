using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    public void LockMovement() {
        GetComponentInParent<PlayerMovement>().LockMovement();
    }

    public void UnlockMovement() {
        GetComponentInParent<PlayerMovement>().UnlockMovement();
    }

    public void SetDownSwords() {     
        GetComponentInParent<PlayerCombat>().SetDownSwords();
    }

    public void PickUpSwords() {
        GetComponentInParent<PlayerCombat>().PickUpSwords();
    }

    public void Hit(int id) {
        GetComponentInParent<PlayerCombat>().Hit(id);
    }

    public void OnSheath() {
        GetComponentInParent<PlayerCombat>().OnSheath();
    }

    public void Step() {
        GameManager.Instance.GetAudioManager().PlayStep();
    }

    public void EndHit() {
        GetComponentInParent<PlayerCombat>().EndHit();
    }

    public void Jump() {
        GetComponentInParent<PlayerMovement>().Jump();
    }

    public void UseManastone() {
        GetComponentInParent<PlayerInteractions>().UseManastone();
    }
}
