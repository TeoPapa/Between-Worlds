using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public int ManastoneEnergy = 100;
    Interaction CurrentInteraction;
    Manastone CurrentManastone;
    Animator PlayerAnimations;

    private void Start() {
        PlayerAnimations = this.GetComponentInChildren<Animator>();
    }

    void OnInteract() {
        if (CurrentInteraction == null || GameHandler.LockedInput) return;
        CurrentInteraction.Interact();
    }

    void OnManastone() {
        if (GameHandler.LockedInput) return;
        PlayerAnimations.SetTrigger("Manastone");
    }

    void OnPause() {
        UIManager UI = GameManager.Instance.GetUIManager();
        if (UI.isPaused())
            UI.ClosePauseMenu();
        else
            UI.OpenPauseMenu();
    }

    public void UseManastone() {
        if (CurrentManastone == null) return;

        CurrentManastone.Activate();
    }

    public void SetInteraction(Interaction i) {
        CurrentInteraction = i;
    }

    public void SetManastone(Manastone m) {
        CurrentManastone = m;
    }

    public bool isCurrent(Interaction i) {
        return CurrentInteraction == i;
    }

    public bool isCurrentMana(Manastone m) {
        return CurrentManastone == m;
    }
}
