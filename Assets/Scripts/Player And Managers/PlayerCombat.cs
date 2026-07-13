using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour {
    Animator PlayerAnimations;

    public int MaxHealth;
    int CurrentHealth;

    public float AttackRange;
    public float DefendRange;

    public int MainDamage;
    public int ShadowDamage;

    public LayerMask EnemyLayer;

    public Transform AttackPoint;
    public Transform DefendPoint;

    public GameObject MainSword;

    public Transform PelvisLeft;
    public Vector3 PelvisPosition;
    public Vector3 PelvisRotation;

    public Transform RightHand;
    public Vector3 RightHandPosition;
    public Vector3 RightHandRotation;

    public float DefCoolDown;
    private float CurrentDefCoolDown;

    public bool CombatLock = false;
    public bool ShadowLock = false;

    bool Sheathed;

    bool CanHit;
    bool Defending;


    private void Start() {
        GameManager.Instance.GetUIManager().UpdateHealthBar(CurrentHealth, MaxHealth);
        Sheathed = true;
        CurrentDefCoolDown = DefCoolDown;
        CanHit = true;
        Defending = false;
        PlayerAnimations = this.GetComponentInChildren<Animator>();
        CurrentHealth = MaxHealth;
    }

    public void PickUpSwords() {
        SetSword(RightHand, RightHandPosition, RightHandRotation);
    }

    public void SetDownSwords() {
        SetSword(PelvisLeft, PelvisPosition, PelvisRotation);
    }

    void SetSword(Transform o, Vector3 pos, Vector3 rot) {
        MainSword.transform.SetParent(o);
        MainSword.transform.localPosition = pos;
        MainSword.transform.localRotation = Quaternion.Euler(rot);
    }

    public void OnSheath() {
        if (!GameManager.Instance.GetMovement().CheckMovement() || CombatLock) {
            return;
        }

        if (Sheathed) {
            PlayerAnimations.SetTrigger("Unsheath");
            GameManager.Instance.GoToCombat();
        } else {
            PlayerAnimations.SetTrigger("Sheath");
            GameManager.Instance.ReturnFromCombat();
        }
        Sheathed = !Sheathed;

        PlayerAnimations.SetBool("IsSheathed", Sheathed);
    }

    void OnAttack() {
        if (EngageCombat()) return;

        if (Sheathed) {
            OnSheath();
            return;
        }
        CanHit = false;
        PlayerAnimations.SetTrigger("Attack");
    }

    public void Hit(int id) {
        int Amount = MainDamage;
        Transform Point = AttackPoint;
        float Range = AttackRange;

        if (id == 1) {
            Amount = ShadowDamage;
        }

        Collider[] HitColliders = PlayerCombatMove(Point, Range);

        if(HitColliders.Length == 0) {
            GameManager.Instance.GetAudioManager().PlayCombatEffect(1);
            return;
        }

        GameManager.Instance.GetAudioManager().PlayCombatEffect(0);
        foreach (Collider en in PlayerCombatMove(Point, Range)) {
            if (id == 0 && en.gameObject.tag == "Nightmare")
                Amount = 0;
            else if(id == 1 && en.gameObject.tag == "Horror")
                Amount = ShadowDamage/4;

            en.GetComponent<Enemy>().TakeDamage(Amount);
        }
    }

    public void TakeHit(Enemy Attacker) {
        if (!Defending) {
            CurrentHealth -= Attacker.Damage;
            return;
        }

        foreach (Collider en in PlayerCombatMove(DefendPoint, DefendRange)) {
            if(en.gameObject == Attacker.gameObject && en.gameObject.tag == "Horror") {
                GameManager.Instance.GetAudioManager().PlayCombatEffect(2);
                en.GetComponent<Enemy>().TakeDamage(Attacker.Damage);
                return;
            }
        }

        if(!Sheathed)
            OnSheath();
    }

    Collider[] PlayerCombatMove(Transform Point, float Range) {
        return Physics.OverlapSphere(Point.position, Range, EnemyLayer);
    }

    public void EndHit() {
        CanHit = true;
    }


    void OnSpecialAttack() {
        if (EngageCombat() || ShadowLock) return;


        CanHit = false;

        PlayerAnimations.SetTrigger("Shadow");
    }

    void OnDefend() {
        if (EngageCombat()) return;

        CanHit = false;
        Defending = true;
        PlayerAnimations.SetTrigger("Deflect");
        CurrentDefCoolDown = 0;
    }

    void OnEndDefend() {
        CurrentDefCoolDown = DefCoolDown;
    }

    bool EngageCombat() {
        bool ec = Sheathed || !CanHit;

        if (Sheathed)
            OnSheath();
        
        return ec;
    }

    void EndDeflect() {
        PlayerAnimations.SetTrigger("EndDeflect");
        Defending = false;
        CanHit = true;
    }

    void Update() {
        if (CurrentDefCoolDown < DefCoolDown) {
            CurrentDefCoolDown += Time.deltaTime;
            return;
        }

        if (CurrentDefCoolDown >= DefCoolDown && Defending)
            EndDeflect();
        
    }

    private void FixedUpdate() {
        if(CurrentHealth <= 0) {
            Death();
            return;
        }

        if (CurrentHealth >= MaxHealth)
            GameManager.Instance.GetUIManager().HideHealthBar();
        else
            GameManager.Instance.GetUIManager().UpdateHealthBar(CurrentHealth, MaxHealth);
    }

    void Death() {
        PlayerAnimations.SetTrigger("Death");
        GameManager.Instance.GetMovement().LockInput();
        GameManager.Instance.GetUIManager().PlayerDeathScreen();
    }

    private void OnDrawGizmosSelected() {
        if(AttackPoint == null || DefendPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(DefendPoint.position, DefendRange);
    }

}
