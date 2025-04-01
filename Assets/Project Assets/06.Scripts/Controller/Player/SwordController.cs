using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public int canAttack = 1;
    private float weaponDamage = 1;

    private void Start()
    {
        SetWeaponDamage(5);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Contains("Enemy") && canAttack == 1)
        {
            col.transform.GetComponent<EnemyStatController>().TakeDamage(weaponDamage);
        }
    }

    public void SetWeaponDamage(float dmg)
    {
        weaponDamage = dmg;
    }
}
