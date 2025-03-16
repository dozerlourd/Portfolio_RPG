using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public int canAttack = 1;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Contains("Enemy") && canAttack == 1)
        {
            print("Damage");

        }
    }
}
