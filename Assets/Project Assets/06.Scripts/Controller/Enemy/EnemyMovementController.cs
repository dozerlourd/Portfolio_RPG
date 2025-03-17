using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    private void EnemyMove()
    {
        StartCoroutine(MoveAIRoutine());
    }

    IEnumerator MoveAIRoutine()
    {
        yield return null;
    }
}
