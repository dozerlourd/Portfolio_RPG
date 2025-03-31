using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatController : MonoBehaviour
{
    [SerializeField] private float hp = 100;
    [SerializeField] EnemyMainController enemyMainController;

    public float HP => hp;

    public void RecoveryHP(float recoveryValue)
    {
        hp -= recoveryValue;
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            enemyMainController.EnemyAnimationController.PlayDyingAnimation();
        }
    }
}
