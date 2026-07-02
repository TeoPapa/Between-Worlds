using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    public List<AudioClip> Steps;

    public List<AudioClip> Hits;

    public void Footstep() {
        GameManager.Instance.GetAudioManager().PlayEnemySteps(Steps[Random.Range(0, Steps.Count - 1)]);
    }

    public void Attack() {
        AudioSource.PlayClipAtPoint(Hits[Random.Range(0, Hits.Count - 1)], transform.position);
        GetComponentInParent<Enemy>().Hit();
    }

    public void Die() {
        GetComponentInParent<Enemy>().DestroyMe();
    }
}
