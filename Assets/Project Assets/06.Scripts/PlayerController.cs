using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    CharacterController characterController;
    Transform characterTr;

    [HideInInspector] public int isAnimEnd = 0;
    bool isBehaviour = false;
    bool isGrounded = false;

    Coroutine Co_blockRoutine;
    Coroutine Co_attackRoutine;
    Coroutine Co_jumpRoutine;

    void Awake()
    {
        characterTr = transform;
        anim = characterTr.GetComponent<Animator>();
        characterController = characterTr.GetComponent<CharacterController>();
    }

    private void Start()
    {
        anim.SetBool("IsGrounded", true);
        isGrounded = true;
    }

    void Update()
    {
        if (isGrounded && !isBehaviour)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) //Attack
            {
                Co_attackRoutine = StartCoroutine(AttackRoutine());
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame) //Block
            {
                Co_blockRoutine = StartCoroutine(ShieldBlockingRoutine());
            }
            else if (Keyboard.current.spaceKey.wasPressedThisFrame) //Jump
            {
                Co_jumpRoutine = StartCoroutine(JumpRoutine());
            }
        }
    }

    IEnumerator ShieldBlockingRoutine()
    {
        isAnimEnd = 0;
        isBehaviour = true;

        anim.SetTrigger("ToBlocking");

        yield return new WaitUntil(() => isAnimEnd == 1);
        isBehaviour = false;
    }

    IEnumerator AttackRoutine()
    {
        isAnimEnd = 0;
        isBehaviour = true;

        int attackNum = Random.Range(0, 3);

        anim.SetTrigger($"ToAttack_{attackNum + 1}");

        yield return new WaitUntil(() => isAnimEnd == 1);
        isBehaviour = false;
    }

    IEnumerator JumpRoutine()
    {
        isAnimEnd = 0;
        isBehaviour = true;

        anim.SetTrigger("ToJump");

        yield return new WaitUntil(() => isAnimEnd == 1);
        isBehaviour = false;
    }

    public void SetAnimEnd(int num) => isAnimEnd = num;
}
