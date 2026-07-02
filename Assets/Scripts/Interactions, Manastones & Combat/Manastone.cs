using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Manastone : MonoBehaviour
{
    public ManastoneItem Item;

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;

        GameManager.Instance.GetInteractions().SetManastone(this);
    }

    private void OnTriggerExit(Collider other) {
        if(!other.CompareTag("Player")) return;
        PlayerInteractions playerInteractions = GameManager.Instance.GetInteractions();

        if(playerInteractions.isCurrentMana(this)) {
            playerInteractions.SetManastone(null);
        }
    }

    public void Activate() {
        Item.Activate();
    }
}
