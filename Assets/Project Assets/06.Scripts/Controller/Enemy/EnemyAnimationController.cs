using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator;
    [SerializeField] EnemyMainController enemyMainController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(HandleEnemyAnimation());
    }

    IEnumerator HandleEnemyAnimation()
    {
        yield return new WaitUntil(() => enemyMainController.EnemyMovementController != null || animator != null);

        while (true)
        {
            yield return new WaitUntil(() => !enemyMainController.IsAttacking && enemyMainController.AnimationIsEnd == 1);


            switch (enemyMainController.EnemyMovementController.currentState)
            {
                case EnemyState.Patrol:
                    animator.SetBool("IsWalking", true);
                    animator.SetFloat("WalkSpeed", 0.75f);
                    animator.SetBool("IsIdle", false);
                    break;
                case EnemyState.Chase:
                    animator.SetBool("IsWalking", true);
                    animator.SetFloat("WalkSpeed", 1.2f);
                    animator.SetBool("IsIdle", false);
                    break;
                case EnemyState.Alert:
                    animator.SetBool("IsWalking", false);
                    animator.SetBool("IsIdle", true);
                    break;
                case EnemyState.Attack:
                    int attackAnim = Random.Range(1, 4); // 1, 2, 3

                    animator.SetBool("IsWalking", false);
                    animator.SetBool("IsIdle", false);
                    animator.SetTrigger($"ToAttack{attackAnim}");
                    enemyMainController.SetIsAttacking(true);
                    yield return new WaitForSeconds(2f);
                    enemyMainController.SetIsAttacking(false);
                    break;
            }
        }
    }
    
    public void PlayDyingAnimation()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsDead", true);
        animator.SetTrigger("ToDying");
    }
}