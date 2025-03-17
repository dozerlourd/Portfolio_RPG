using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public Transform target; // 이동 목표 연결
    public float speed = 5f;
    public float nextWaypointDistance = 3f;

    List<Node> path;
    int currentWaypoint = 0;
    AStar aStar;

    void Start()
    {
        aStar = FindObjectOfType<AStar>();
        UpdatePath();
    }

    void Update()
    {
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.position) > nextWaypointDistance)
            {
                UpdatePath();
            }
        }

        if (path == null || path.Count == 0)
        {
            return;
        }

        if (currentWaypoint >= path.Count)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, path[currentWaypoint].worldPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, path[currentWaypoint].worldPosition) < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void UpdatePath()
    {
        path = aStar.FindPath(transform.position, target.position);
        currentWaypoint = 0;
    }
}