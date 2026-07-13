using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Character))]
public class PlayerMovement : MonoBehaviour {
    CharacterController Contr;
    public Transform Camera;

    Animator PlayerAnimations;

    public float WalkingSpeed;
    public float RunningSpeed;
    public float TurnSmooth = 0.1f;

    public float JumpForce;
    public float JumpWalk;
    public float JumpRun;

    public float Gravity = -9.81f;

    public LayerMask GroundLayer;
    public Transform GroundPoint;

    float MovementX;
    float MovementY;
    float CurrentSpeed;
    float CurrentJumpSpeed;

    bool IsGrounded;

    Vector3 Velocity;

    float TurnSmoothVelocity;

    bool CanMove = true;

    private bool LockedInput;
    public GameObject CurrentCamera;

    [HideInInspector]
    public bool isSitting = false;

    public bool Teleport = false;

    float PrevY;

    private void Awake() {
        Teleport = true;

        LockedInput = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start() {
        Contr = this.GetComponent<CharacterController>();
        CurrentSpeed = WalkingSpeed;
        CurrentJumpSpeed = JumpWalk;
        PlayerAnimations =  this.GetComponentInChildren<Animator>();
        PlayerAnimations.SetBool("IsRunning", false);
        PlayerAnimations.SetFloat("Speed", 0);
        PrevY = transform.position.y;
    }


    public void UnlockInput() {
        LockedInput = false;
        CurrentCamera.GetComponent<CinemachineInputAxisController>().enabled = true;
    }

    public void LockInput() {
        LockedInput = true;
        CurrentCamera.GetComponent<CinemachineInputAxisController>().enabled = false;
    }

    public void UnlockMovement() {
        CanMove = true;
    }

    public void LockMovement() {
        CanMove = false;
    }

    void OnMove(InputValue movementValue) {
        if (LockedInput)
            return;

        if (isSitting) {
            Stand();
            return;
        }
        Vector2 moveVector = movementValue.Get<Vector2>();
        MovementX = moveVector.x;
        MovementY = moveVector.y;
    }

    void OnSprint() {
        if (LockedInput) return;

        CurrentSpeed = RunningSpeed;
        CurrentJumpSpeed = JumpRun;
        PlayerAnimations.SetBool("IsRunning", true);
    }

    void OnWalk() {
        if (LockedInput) return;

        CurrentSpeed = WalkingSpeed;
        CurrentJumpSpeed = JumpWalk;
        PlayerAnimations.SetBool("IsRunning", false);
    }

    void OnJump() {
        if (LockedInput || !CanMove) return;

        if (isSitting) {
            Stand();
            return;
        }
        CanMove = false;
        PlayerAnimations.SetTrigger("Jump");
    }

    public void Jump() {
        CanMove = true;
        Velocity.y = Mathf.Sqrt(JumpForce * -1f * Gravity);
    }

    public bool CheckMovement() {
        return CanMove && IsGrounded && (!LockedInput);
    }

    public void Sit(float rotation) {
        this.transform.rotation = Quaternion.Euler(0, rotation, 0);
        PlayerAnimations.SetTrigger("Sit");
        isSitting = true;
    }

    public void Stand() {
        if (!isSitting) return;
        PlayerAnimations.SetTrigger("Stand");
        isSitting = false;
    }

    public bool isLocked() {
        return LockedInput;
    }

    private void Update() {
        if (LockedInput) {
            MovementX = 0;
            MovementY = 0;
        }
        IsGrounded = Physics.OverlapSphere(GroundPoint.position, .2f, GroundLayer).Length > 0;

        PlayerAnimations.SetBool("IsGrounded", IsGrounded);
        if (CanMove) {
            if (IsGrounded && Velocity.y < 0) {
                Velocity.y = -1f;
            }
        }

        if (isSitting) return;
        if (MovementX == 0 && MovementY == 0) {
            PlayerAnimations.SetFloat("Speed", 0);
        } else {
            PlayerAnimations.SetFloat("Speed", 1f);
        }
    }

    void FixedUpdate() {

        if (Teleport) {
            this.gameObject.transform.position = GameHandler.PlayerPosition;
            this.gameObject.transform.rotation = Quaternion.Euler(GameHandler.PlayerRotation);
            Teleport = false;
            return;
        }

        if (isSitting || LockedInput) {
            return;
        }

        Vector3 move = new Vector3(MovementX, 0.0f, MovementY).normalized;


        float Angle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
        float smoothed = Mathf.SmoothDampAngle(transform.eulerAngles.y, Angle, ref TurnSmoothVelocity, TurnSmooth);
        transform.rotation = Quaternion.Euler(0f, smoothed, 0f);

        if (move.magnitude >= 0.1f && CanMove) {
            Vector3 moveDir = Quaternion.Euler(0f, Angle, 0f) * Vector3.forward;
            float Speed = CurrentSpeed;
            if (!IsGrounded) Speed = CurrentJumpSpeed;
            Contr.Move(moveDir.normalized * Speed * Time.deltaTime);
        }
        Velocity.y += Gravity * Time.deltaTime;
            Contr.Move(Velocity * Time.deltaTime);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(GroundPoint.position, .2f);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(GroundPoint.position, GroundPoint.position+Vector3.up * 0.1f);
    }
}
