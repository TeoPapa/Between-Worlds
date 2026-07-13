using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AudioArea : MonoBehaviour
{
    [SerializeField] public int MusicIndex;
    [SerializeField] public bool Skip;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            
            GameManager.Instance.ChangeMusicId(MusicIndex, !Skip);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.ChangeMusicId(MusicIndex+1, !Skip);
        }
    }
}
