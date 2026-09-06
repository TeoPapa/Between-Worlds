using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmbientArea : SoundfulObject {
    public AudioClip AmbientClip;

    private void Start() {
        DefaultSource.loop = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;

        PlaySound(AmbientClip);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) return;
        StopSound();
    }
}
