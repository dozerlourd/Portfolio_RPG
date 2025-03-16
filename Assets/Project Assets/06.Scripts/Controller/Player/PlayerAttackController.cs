using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackController : MonoBehaviour
{
    Animator anim;
    PlayerStatController playerStatController;
    public SwordController swordController;
    private Collider swordCollider;

    public Coroutine Co_attackRoutine { get; private set; }

    void Awake()
    {
        anim = GetComponent<Animator>();
        swordCollider = swordController.GetComponent<Collider>();
    }

    void Start()
    {
        swordCollider.enabled = false;
    }

    public void HandleAttack()
    {
        if (playerStatController.IsAnimEnd == 1)
        {
            if (Co_attackRoutine != null) {
                StopCoroutine(Co_attackRoutine);
            }
            Co_attackRoutine = StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        swordCollider.enabled = true;
        playerStatController.IsAnimEnd = 0;
        playerStatController.CanNextBehaviour = false;

        int attackNum = Random.Range(0, 4);
        anim.SetTrigger($"ToAttack_{attackNum + 1}");

        if (attackNum + 1 == 4)
        {
            StartCoroutine(AttackForwardMovement());
        }

        yield return new WaitUntil(() => playerStatController.IsAnimEnd == 1);
        anim.SetBool("IsMove", CheckMoveInput());
        yield return new WaitForSeconds(0.35f);
        playerStatController.CanNextBehaviour = true;
        swordCollider.enabled = false;
    }

    IEnumerator AttackForwardMovement()
    {
        yield return new WaitForSeconds(0.06f);

        float elapsedTime = 0f;
        float duration = 0.35f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + transform.forward * 0.75f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
            yield return null;
        }

        elapsedTime = 0f;
        duration = 0.3f;
        startPosition = transform.position;
        targetPosition = transform.position + transform.forward * 2f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
            yield return null;
        }

        transform.position = targetPosition;
    }

    bool CheckMoveInput()
    {
        bool isWPressed = Keyboard.current.wKey.isPressed;
        bool isAPressed = Keyboard.current.aKey.isPressed;
        bool isSPressed = Keyboard.current.sKey.isPressed;
        bool isDPressed = Keyboard.current.dKey.isPressed;

        return isWPressed || isAPressed || isSPressed || isDPressed;
    }

    public void SetAnimEnd(int num) => playerStatController.IsAnimEnd = num;
    public void SetCanAttack(int num) => swordController.canAttack = num;

    public void SetPlayerStatController(PlayerStatController pVariables) => playerStatController = pVariables;
}