using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    CharacterController characterController;
    Transform characterTr;

    private InputAction moveAction;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private SwordController swordController;
    private Collider swordCollider;

    [SerializeField] private float playerHalfLength;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 9.81f;

    private Vector3 smoothedDirection;
    private Vector3 camForward;
    private Vector3 camRight;

    private float currentRunningSpeed = 0;
    private float currentMoveSpeed = 0f;
    private float acceleration = 10f;
    private float deceleration = 20f;
    private float velocityRef = 0f;     // Reference value of SmoothDamp

    int attackNum = 0;

    private int isAnimEnd = 1;
    private int isNotSpecificAnimation = 1;
    private bool canNextBehaviour = true;
    private bool isGrounded = false;

    Coroutine Co_blockRoutine;
    Coroutine Co_attackRoutine;
    Coroutine Co_jumpRoutine;

    public Vector3 inputDirection { get; private set; }

    void Awake()
    {
        characterTr = transform;
        anim = characterTr.GetComponent<Animator>();
        characterController = characterTr.GetComponent<CharacterController>();
        swordCollider = swordController.GetComponent<Collider>();
    }

    private void Start()
    {
        swordCollider.enabled = false;
    }

    void Update()
    {
        isGrounded = CheckToGrounded();
        anim.SetBool("IsGrounded", isGrounded);

        GetCameraDirectionExceptY();
        if (isGrounded && canNextBehaviour)
        {
            HandleCharacterActions();
            UpdateMovement();
        }
    }

    private void UpdateMovement()
    {
        bool isMoving = isNotSpecificAnimation == 1 && inputDirection.magnitude > 0f;
        bool canMovingAnimation = isAnimEnd == 1 && inputDirection.magnitude > 0f;

        if (isMoving)
        {
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, moveSpeed + currentRunningSpeed, ref velocityRef, 1f / acceleration);
        }
        else
        {
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0f, ref velocityRef, 1f / deceleration);
        }

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

        smoothedDirection = Vector3.Lerp(smoothedDirection, inputDirection.normalized * currentMoveSpeed, Time.deltaTime * 10f);

        anim.SetFloat("Horizontal", smoothedDirection.x);
        anim.SetFloat("Vertical", smoothedDirection.z);
    }

    private void GetCameraDirectionExceptY()
    {
        camForward = cameraTransform.forward;
        camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        inputDirection = new Vector3(input.x, 0, input.y);
    }

    bool CheckToGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHalfLength, LayerMask.GetMask("Ground"));
    }

    void MovePlayer()
    {
        Vector3 movement = (camForward * inputDirection.z + camRight * inputDirection.x).normalized * currentMoveSpeed * Time.deltaTime;
        characterController.Move(movement);

        if (movement.magnitude > 0 && movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        anim.SetFloat("Horizontal", inputDirection.normalized.x);
        anim.SetFloat("Vertical", inputDirection.normalized.z);
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
        canNextBehaviour = false;
        anim.SetTrigger("ToBlocking");

        yield return new WaitUntil(() => isAnimEnd == 1);
        canNextBehaviour = true;
    }

    IEnumerator AttackRoutine()
    {
        swordCollider.enabled = true;
        isAnimEnd = 0;
        canNextBehaviour = false;

        //int attackNum = Random.Range(0, 4);
        int attackNum = 3;
        anim.SetTrigger($"ToAttack_{attackNum + 1}");
        
        if (attackNum + 1 == 4)
        {
            StartCoroutine(AttackForwardMovement());
        }

        yield return new WaitUntil(() => isAnimEnd == 1);
        //attackNum = ((attackNum + 1) % 4);
        canNextBehaviour = true;
        swordCollider.enabled = false;
    }

    IEnumerator AttackForwardMovement()
    {
        yield return new WaitForSeconds(0.06f);

        float elapsedTime = 0f;
        float duration = 0.35f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + transform.forward * 0.75f; // Move forward

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // smoothly increase from 0 to 1

            // 비등속 전진 (Ease-out 효과)
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);

            yield return null;
        }

        elapsedTime = 0f;
        duration = 0.3f;
        startPosition = transform.position;
        targetPosition = transform.position + transform.forward * 2f; // Move forward

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // smoothly increase from 0 to 1

            // 비등속 전진 (Ease-out 효과)
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);

            yield return null;
        }

        transform.position = targetPosition; // Move to the correct target position
    }

    IEnumerator JumpRoutine()
    {
        isAnimEnd = 0;
        canNextBehaviour = false;
        anim.SetTrigger("ToJump");

        yield return new WaitUntil(() => isAnimEnd == 1);
        canNextBehaviour = true;
    }

    public void SetAnimEnd(int num) => isAnimEnd = num;
    public void SetIsNotSpecificAnimation(int num) => isNotSpecificAnimation = num;
    public void SetCanAttack(int num) => swordController.canAttack = num;
}