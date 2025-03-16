using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    Animator anim;
    PlayerStatController playerStatController;
    CharacterController characterController;
    public Transform cameraTransform;

    [SerializeField] private float playerHalfLength;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 20f;

    private Vector3 smoothedDirection;
    private Vector3 camForward;
    private Vector3 camRight;
    private float currentRunningSpeed = 0;
    private float currentMoveSpeed = 0f;
    private float velocityRef = 0f;
    public Vector3 inputDirection { get; private set; }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    public void UpdateMovement()
    {
        anim.SetBool("IsGrounded", CheckToGrounded());
        GetCameraDirectionExceptY();

        if (!(playerStatController.CanNextBehaviour && (playerStatController.IsAnimEnd == 1))) return;

        bool isMoving = inputDirection.magnitude > 0f && playerStatController.CanNextBehaviour;

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
        UpdateMoveAnimation(isMoving);
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

    public bool CheckToGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHalfLength, LayerMask.GetMask("Ground"));
    }

    private void MovePlayer()
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

    public void SetPlayerStatController(PlayerStatController pVariables) => playerStatController = pVariables;
}