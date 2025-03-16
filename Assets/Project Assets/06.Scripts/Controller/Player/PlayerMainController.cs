using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMainController : MonoBehaviour
{
    [SerializeField] PlayerAnimationController playerAnimationController;
    [SerializeField] PlayerMovementController playerMovementController;
    [SerializeField] PlayerAttackController playerAttackController;

    [SerializeField] PlayerStatController playerStatController;

    private void Start()
    {
        playerAnimationController.SetPlayerStatController(playerStatController);
        playerMovementController.SetPlayerStatController(playerStatController);
        playerAttackController.SetPlayerStatController(playerStatController);
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
