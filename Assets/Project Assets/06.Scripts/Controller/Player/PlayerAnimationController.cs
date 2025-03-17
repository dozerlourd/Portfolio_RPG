using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    Animator anim;
    PlayerMainController playerMainController;

    public Coroutine Co_blockRoutine { get; private set; }
    public Coroutine Co_jumpRoutine { get; private set; }

    private void Awake() => anim = GetComponent<Animator>();

    public void HandleJump()
    {
        if (playerMainController.CanNextBehaviour)
        {
            Co_jumpRoutine = StartCoroutine(JumpRoutine());
        }
    }

    public void HandleBlock()
    {
        if (playerMainController.CanNextBehaviour)
        {
            Co_blockRoutine = StartCoroutine(ShieldBlockingRoutine());
        }
    }

    IEnumerator ShieldBlockingRoutine()
    {
        anim.SetBool("IsMove", false);
        playerMainController.IsAnimEnd = 0;
        playerMainController.CanNextBehaviour = false;
        anim.SetTrigger("ToBlocking");
        yield return new WaitUntil(() => playerMainController.IsAnimEnd == 1);
        yield return new WaitForSeconds(0.3f);
        playerMainController.CanNextBehaviour = true;
    }

    IEnumerator JumpRoutine()
    {
        playerMainController.IsAnimEnd = 0;
        playerMainController.CanNextBehaviour = false;
        anim.SetTrigger("ToJump");
        yield return new WaitUntil(() => playerMainController.IsAnimEnd == 1);
        yield return new WaitForSeconds(0.3f);
        playerMainController.CanNextBehaviour = true;
    }

    public void SetPlayerMainController(PlayerMainController pmController) => playerMainController = pmController;

    public void SetAnimEnd(int num) => playerMainController.IsAnimEnd = num;
}