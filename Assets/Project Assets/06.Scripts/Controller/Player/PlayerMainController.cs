using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMainController : MonoBehaviour
{
    [SerializeField] PlayerAnimationController playerAnimationController;
    [SerializeField] PlayerMovementController playerMovementController;
    [SerializeField] PlayerAttackController playerAttackController;

    private bool canNextBehaviour = true;
    private int isAnimEnd = 1;

    public bool CanNextBehaviour { get => canNextBehaviour; set => canNextBehaviour = value; }
    public int IsAnimEnd { get => isAnimEnd; set => isAnimEnd = value; }

    private void Start()
    {
        playerAnimationController.SetPlayerMainController(this);
        playerMovementController.SetPlayerMainController(this);
        playerAttackController.SetPlayerMainController(this);
    }

    private void Update()
    {
        if (playerMovementController.CheckToGrounded())
        {
            playerMovementController.UpdateMovement();
            HandleCharacterActions();
        }
    }

    private void HandleCharacterActions()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            playerAttackController.HandleAttack();
        else if (Mouse.current.rightButton.wasPressedThisFrame)
            playerAnimationController.HandleBlock();
        else if (Keyboard.current.spaceKey.wasPressedThisFrame)
            playerAnimationController.HandleJump();
    }
}
