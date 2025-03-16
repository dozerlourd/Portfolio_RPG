using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    Animator anim;
    PlayerStatController playerStatController;

    public Coroutine Co_blockRoutine { get; private set; }
    public Coroutine Co_jumpRoutine { get; private set; }

    private void Awake() => anim = GetComponent<Animator>();

    public void HandleJump()
    {
        if (playerStatController.CanNextBehaviour)
        {
            Co_jumpRoutine = StartCoroutine(JumpRoutine());
        }
    }

    public void HandleBlock()
    {
        if (playerStatController.CanNextBehaviour)
        {
            Co_blockRoutine = StartCoroutine(ShieldBlockingRoutine());
        }
    }

    IEnumerator ShieldBlockingRoutine()
    {
        anim.SetBool("IsMove", false);
        playerStatController.IsAnimEnd = 0;
        playerStatController.CanNextBehaviour = false;
        anim.SetTrigger("ToBlocking");
        yield return new WaitUntil(() => playerStatController.IsAnimEnd == 1);
        yield return new WaitForSeconds(0.3f);
        playerStatController.CanNextBehaviour = true;
    }

    IEnumerator JumpRoutine()
    {
        playerStatController.IsAnimEnd = 0;
        playerStatController.CanNextBehaviour = false;
        anim.SetTrigger("ToJump");
        yield return new WaitUntil(() => playerStatController.IsAnimEnd == 1);
        yield return new WaitForSeconds(0.3f);
        playerStatController.CanNextBehaviour = true;
    }

    public void SetPlayerStatController(PlayerStatController pVariables) => playerStatController = pVariables;

    public void SetAnimEnd(int num) => playerStatController.IsAnimEnd = num;
}