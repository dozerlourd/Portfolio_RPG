using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [SerializeField] private int hp = 100;
    private bool canNextBehaviour = true;
    private int isAnimEnd = 1;

    public bool CanNextBehaviour { get => canNextBehaviour; set => canNextBehaviour = value; }
    public int IsAnimEnd { get => isAnimEnd; set => isAnimEnd = value; }
    public int HP => hp;

    public void RecoveryHP(int recoveryValue)
    {
        hp -= recoveryValue;
    }

    public void PlayerDamaged(int dmg)
    {
        hp -= dmg;
    }
}
