using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmbientArea : MonoBehaviour
{
    public int AmbientClip;

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;

        GameManager.Instance.GetAudioManager().EnterAmbience(AmbientClip);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.GetAudioManager().ExitAmbience();
    }
}
