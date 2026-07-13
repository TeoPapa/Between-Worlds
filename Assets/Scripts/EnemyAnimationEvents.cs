using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    public void Footstep() {
        GetComponentInParent<Enemy>().MakeAFootstep();
    }

    public void Attack() {
        GetComponentInParent<Enemy>().Hit();
    }

    public void Die() {
        GetComponentInParent<Enemy>().DestroyMe();
    }
}
