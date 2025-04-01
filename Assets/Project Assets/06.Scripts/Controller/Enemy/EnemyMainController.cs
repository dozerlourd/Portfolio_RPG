using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMainController : MonoBehaviour
{
    [SerializeField] EnemyStatController enemyStatController;
    [SerializeField] EnemyAttackController enemyAttackController;
    [SerializeField] EnemyAnimationController enemyAnimationController;
    [SerializeField] EnemyMovementController enemyMovementController;

    int animationIsEnd = 1;
    bool isAttacking = false;

    bool isDead = false;

    public int AnimationIsEnd => animationIsEnd;
    public bool IsAttacking => isAttacking;
    public bool IsDead => isDead;

    public EnemyStatController EnemyStatController => enemyStatController;
    public EnemyAttackController EnemyAttackController => enemyAttackController;
    public EnemyAnimationController EnemyAnimationController => enemyAnimationController;
    public EnemyMovementController EnemyMovementController => enemyMovementController;

    public void SetAnimationIsEnd(int num) => animationIsEnd = num;
    public void SetIsAttacking(bool isTrue) => isAttacking = isTrue;
}
