using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordControler : MonoBehaviour
{


    private void OnTriggerEnter(Collider col)
    {
        print(col.gameObject.name);
        if (col.gameObject.tag.Contains("Enemy"))
        {
            print("Damage");
        }
    }
}
