using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    public float attackDelay = 1.5f; // Attack Delay
    public float attackRange = 2.5f; // Attack Detect Range
    public int attackDamage = 10; // Damage

    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    public EnemyAnimationController animationController;
    public EnemyMainController enemyMainController;

    Coroutine Co_CheckAttackDelayRoutine;

    void Update()
    {
        if (enemyMainController.IsDead)
        {
            if(Co_CheckAttackDelayRoutine != null)
            {
                StopCoroutine(Co_CheckAttackDelayRoutine);
            }

            return;
        }

        if (isAttacking)
        {
            return;
        }
    }

    public void EnemyAttack()
    {
        if (Time.time - lastAttackTime < attackDelay)
        {
            return;
        }

        Co_CheckAttackDelayRoutine = StartCoroutine(CheckAttackDelayRoutine());
    }

    IEnumerator CheckAttackDelayRoutine()
    {
        isAttacking = true;

        if (animationController != null)
        {
            // animationController.animator.SetTrigger("Attack");
        }

        //Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attackRange, LayerMask.GetMask("Player"));

        //foreach (Collider hitCollider in hitColliders)
        //{
        //    if (hitCollider.CompareTag("Player"))
        //    {
        //        PlayerStatController playerStats = hitCollider.GetComponent<PlayerStatController>();
        //        if (playerStats != null)
        //        {
        //            playerStats.TakeDamage(attackDamage);
        //        }
        //    }
        //}

        lastAttackTime = Time.time;
        isAttacking = false;
        yield return null;
    }
}