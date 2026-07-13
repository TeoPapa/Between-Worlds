using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : ObjectID
{
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

    int SoundID = -1;

    bool CanAttack;
    bool IsMoving;
    bool CanMove;
    bool PlayingSound;

    Transform Player;
    CharacterController Controller;
    Animator Animations;

    Vector3 Velocity;

    public Enemy(string id) : base(id) {
    }

    public void UnlockMove() {
        CanMove = true;
    }



    void EngageSound() {
        if(PlayingSound || IsMoving || (!IsMoving && !CanMove)) return;

        AudioManager Manager = GameManager.Instance.GetAudioManager();

        SoundID = Manager.FirstEnemySound();
        Manager.PlayEnemySound(IdleSounds[Random.Range(0, IdleSounds.Count - 1)], SoundID, true);

        PlayingSound = true;
    }

    public void MakeAFootstep() {
        GameManager.Instance.GetAudioManager().PlayEnemySound(Steps[Random.Range(0, Steps.Count - 1)], SoundID,false);
    }

    void StopSound() {
        if(!PlayingSound) return;
        GameManager.Instance.GetAudioManager().StopEnemySound(SoundID);
        PlayingSound = false;
    }

    private void Awake() {
        DeathParticles.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    void Update()
    {
        if (CurrentHealth <= 0) {
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
        }
        else {
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
            StopSound();
            IsMoving = true;
            Animations.SetBool("IsMoving", true);
            return;
        }
        
        IsMoving = false;
        Animations.SetBool("IsMoving", false);
    }

    private void FixedUpdate() {
        if(IsMoving && CanMove) {
           transform.LookAt(Player);
           
            Vector3 direction = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * Vector3.forward;

            Controller.Move(direction *Speed*Time.deltaTime);
        }

        Velocity.y += Physics.gravity.y * Time.deltaTime;
        Controller.Move(Velocity * Time.deltaTime);
    }

    public void TakeDamage(int Ammount) {
        CurrentHealth -= Ammount;
    }

    public void Hit() {
        Collider[] hitColliders = Physics.OverlapSphere(AttackPoint.position, AttackRange, PlayerLayer);
        foreach (Collider col in hitColliders) {
            col.GetComponent<PlayerCombat>().TakeHit(this);
            GameManager.Instance.GetAudioManager().PlayEnemySound(Hits[Random.Range(0, Hits.Count - 1)], SoundID, false);
            break;
        }
        CanMove = true;
    }

    void Die() {
        GameManager.Instance.GetAudioManager().StopEnemySound(SoundID);
        GameManager.Instance.GetAudioManager().PlayEnemySound(DeathSound, SoundID, false);
        HealthBar.SetActive(false);
        Animations.SetTrigger("Die");
        DeathParticles.SetActive(true);
    }

    public void DestroyMe() {
        if (Story != null) { Story.EngageEvent(); }

        Engaged();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, VisualRange);

        if(AttackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            EngageSound();
        }
    }
}
