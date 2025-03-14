using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    CharacterController characterController;
    Transform characterTr;

    private InputAction moveAction;

    [SerializeField] private float playerHalfLength;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 9.81f;

    private Vector3 smoothedDirection;
    private float currentRunningSpeed = 0;
    private float currentMoveSpeed = 0f;
    private float acceleration = 10f;
    private float deceleration = 20f;
    private float velocityRef = 0f;     // Reference value of SmoothDamp

    private int isAnimEnd = 1;
    private int isNotSpecificAnimation = 1;
    private bool canNextBehaviour = false;
    private bool isGrounded = false;

    Coroutine Co_blockRoutine;
    Coroutine Co_attackRoutine;
    Coroutine Co_jumpRoutine;

    public Vector3 direction { get; private set; }

    void Awake()
    {
        characterTr = transform;
        anim = characterTr.GetComponent<Animator>();
        characterController = characterTr.GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = CheckToGrounded();
        anim.SetBool("IsGrounded", isGrounded);

        if (isGrounded && !canNextBehaviour)
        {
            HandleCharacterActions();
            UpdateMovement();
        }
    }

    private void UpdateMovement()
    {
        bool isMoving = isNotSpecificAnimation == 1 && direction.magnitude > 0f;
        bool canMovingAnimation = isAnimEnd == 1 && direction.magnitude > 0f;

        if (isMoving)
        {
            // 이동 시 가속 적용
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, moveSpeed + currentRunningSpeed, ref velocityRef, 1f / acceleration);
        }
        else if(isNotSpecificAnimation == 1 && (direction.magnitude > 0f || currentMoveSpeed > 0))
        {
            // 키를 떼면 감속 적용
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0f, ref velocityRef, 1f / deceleration);
        }

        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("ani_Paladin_Walking") || anim.GetCurrentAnimatorStateInfo(0).IsName("ani_Paladin_Running"))
        //{
            
        //}
        MovePlayer();
        CheckToRunning();
        UpdateMoveAnimation(canMovingAnimation);
    }

    private void CheckToRunning()
    {
        bool isRunning = Keyboard.current.leftShiftKey.isPressed;
        anim.SetBool("IsRunning", isRunning);
        currentRunningSpeed = isRunning ? runningSpeed : 0;
    }

    private void UpdateMoveAnimation(bool canMovingAnimation)
    {
        anim.SetBool("IsMove", canMovingAnimation);

        smoothedDirection = Vector3.Lerp(smoothedDirection, direction.normalized * currentMoveSpeed, Time.deltaTime * 10f);

        anim.SetFloat("Horizontal", smoothedDirection.x);
        anim.SetFloat("Vertical", smoothedDirection.z);
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        direction = new Vector3(input.x, 0f, input.y);
    }

    bool CheckToGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHalfLength, LayerMask.GetMask("Ground"));
    }

    void MovePlayer()
    {
        Vector3 movement = direction.normalized * currentMoveSpeed * Time.deltaTime;
        characterController.Move(movement);

        anim.SetFloat("Horizontal", direction.normalized.x);
        anim.SetFloat("Vertical", direction.normalized.z);
    }

    private void HandleCharacterActions()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            Co_attackRoutine = StartCoroutine(AttackRoutine());
        else if (Mouse.current.rightButton.wasPressedThisFrame)
            Co_blockRoutine = StartCoroutine(ShieldBlockingRoutine());
        else if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Co_jumpRoutine = StartCoroutine(JumpRoutine());
    }

    IEnumerator ShieldBlockingRoutine()
    {
        isAnimEnd = 0;
        canNextBehaviour = true;
        anim.SetTrigger("ToBlocking");

        yield return new WaitUntil(() => isAnimEnd == 1);
        canNextBehaviour = false;
    }

    IEnumerator AttackRoutine()
    {
        isAnimEnd = 0;
        canNextBehaviour = true;
        anim.SetTrigger($"ToAttack_{Random.Range(1, 4)}");

        yield return new WaitUntil(() => isAnimEnd == 1);
        canNextBehaviour = false;
    }

    IEnumerator JumpRoutine()
    {
        isAnimEnd = 0;
        canNextBehaviour = true;
        anim.SetTrigger("ToJump");

        yield return new WaitUntil(() => isAnimEnd == 1);
        canNextBehaviour = false;
    }

    public void SetAnimEnd(int num) => isAnimEnd = num;
    public void SetIsNotSpecificAnimation(int num) => isNotSpecificAnimation = num;
}