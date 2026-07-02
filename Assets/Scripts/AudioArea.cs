using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AudioArea : MonoBehaviour
{
    [SerializeField] public int MusicIndex;
    [SerializeField] public bool Skip;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.GetAudioManager().PlayMusic(MusicIndex, Skip);

            GameHandler.CurrentMusicID = MusicIndex;
            GameHandler.CurrentMusicRepeats = !Skip;
            GameManager.Instance.Save();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.GetAudioManager().PlayMusic(MusicIndex+1, Skip);

            GameHandler.CurrentMusicID = MusicIndex;
            GameHandler.CurrentMusicRepeats = !Skip;
            GameManager.Instance.Save();
        }
    }
}
