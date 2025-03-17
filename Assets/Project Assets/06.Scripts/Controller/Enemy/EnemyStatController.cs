using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatController : MonoBehaviour
{
    [SerializeField] private float hp = 100;

    public float HP => hp;

    public void RecoveryHP(float recoveryValue)
    {
        hp -= recoveryValue;
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
    }
}
