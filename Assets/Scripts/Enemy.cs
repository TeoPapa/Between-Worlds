using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : SoundfulObject {
    public int Health;
    int CurrentHealth;

    public GameObject HealthBar;
    public Image HealthBarImage;

    public int Damage;

    public float Speed;

    public float VisualRange;
    public float AttackRange;

    public Transform AttackPoint;
    public LayerMask PlayerLayer;

    public Transform GroundPoint;
    public LayerMask GroundLayer;

    public float AttackCoolDown;
    float CurrentAttackCoolDown;

    public List<AudioClip> IdleSounds;

    public List<AudioClip> Steps;

    public List<AudioClip> Hits;

    public AudioClip DeathSound;

    public GameObject DeathParticles;

    public StoryEvent Story;

    bool CanAttack;
    bool IsMoving;
    bool CanMove;
    bool PlayingSound;
    bool IsDead;

    Transform Player;
    CharacterController Controller;
    Animator Animations;

    Vector3 Velocity;

    public void UnlockMove() {
        CanMove = true;
    }

    #region Unity Callbacks

    protected override void SoundfulAwake() {
        DeathParticles.SetActive(false);

        IsDead = false;
    }

    protected override List<AudioSource> SetSources() {
        List<AudioSource> sources = new List<AudioSource>();
        sources.Add(GetComponentInChildren<AudioSource>());
        Volume = 1f;
        return sources;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        PlayingSound = false;
        Player = GameManager.Instance.Player.transform;
        Controller = GetComponent<CharacterController>();
        Animations = GetComponentInChildren<Animator>();
        CurrentHealth = Health;
        CurrentAttackCoolDown = AttackCoolDown;
        CanAttack = true;
        CanMove = true;
    }

    // Update is called once per frame
    void Update() {

        if(IsDead)
            return;
        else if (CurrentHealth <= 0) {
            IsDead = true;
            Die();
            return;
        }

        if (CurrentHealth > Health) CurrentHealth = Health;

        if (CurrentHealth == Health) HealthBar.SetActive(false);
        else {
            HealthBar.SetActive(true);
            HealthBarImage.fillAmount = (float)CurrentHealth / Health;
        }

        if (Physics.OverlapSphere(GroundPoint.position, .2f, GroundLayer).Length > 0) Velocity.y = -1;

        float distance = Vector3.Distance(transform.position, Player.position);

        if (CurrentAttackCoolDown < AttackCoolDown) {
            CanAttack = false;
            CurrentAttackCoolDown += Time.deltaTime;
        } else {
            CanAttack = true;
        }

        if (distance <= AttackRange) {
            IsMoving = false;
            Animations.SetBool("IsMoving", false);
            if (CanAttack) {
                CanMove = false;
                Animations.SetTrigger("Attack");
                CurrentAttackCoolDown = 0;
            }
            return;
        } else if (distance <= VisualRange && CanMove) {
            IsMoving = true;
            Animations.SetBool("IsMoving", true);
            return;
        }

        IsMoving = false;
        Animations.SetBool("IsMoving", false);
    }

    private void FixedUpdate() {
        if (IsMoving && CanMove) {
            transform.LookAt(Player);

            Vector3 direction = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * Vector3.forward;

            Controller.Move(direction * Speed * Time.deltaTime);
        }

        Velocity.y += Physics.gravity.y * Time.deltaTime;
        Controller.Move(Velocity * Time.deltaTime);
    }

    #endregion

    #region Sound
    public void MakeAFootstep() {
        PlaySound(Steps[Random.Range(0, Steps.Count - 1)]);
    }

    #endregion

    #region Damage

    public void TakeDamage(int Ammount) {
        CurrentHealth -= Ammount;
    }

    public void Hit() {
        Collider[] hitColliders = Physics.OverlapSphere(AttackPoint.position, AttackRange, PlayerLayer);
        foreach (Collider col in hitColliders) {
            col.GetComponent<PlayerCombat>().TakeHit(this);
            PlaySound(Hits[Random.Range(0, Hits.Count - 1)]);
            break;
        }
        CanMove = true;
    }

    #endregion

    #region Death

    void Die() {
        CanMove = false;
        CanAttack = false;
        PlaySound(DeathSound);
        HealthBar.SetActive(false);
        Animations.SetTrigger("Die");
        DeathParticles.SetActive(true);
    }

    public void DestroyMe() {
        Debug.Log("Enemy " + gameObject.name + " has been destroyed!");
        if (Story != null) { Story.EngageEvent(); }

        GetComponent<ObjectID>().Engaged();
    }

    #endregion

    #region Unity Events (Triggers, Gizmos etc.)

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, VisualRange);

        if (AttackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlaySound(IdleSounds[Random.Range(0, IdleSounds.Count - 1)]);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            StopSound();
        }
    }

    private void Reset() {
        Debug.Log("Reminder: Attatch an ObjectID component to this (" + gameObject.name + ") object to save its state!");
    }

    #endregion
}
