using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMainController : MonoBehaviour
{
    [SerializeField] EnemyStatController enemyStatController;
    [SerializeField] EnemyAttackController enemyAttackController;
    [SerializeField] EnemyAnimationController enemyAnimationController;
    [SerializeField] EnemyMovementController enemyMovementController;
}
