using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Patrol,
    Alert,
    Chase
}

public class EnemyMovementController : MonoBehaviour
{
    public Transform target;
    public float patrolRadius = 10f;
    public float alertRadius = 5f;
    public float chaseRadius = 3f;
    public float speed = 5f;
    public float nextWaypointDistance = 1f;

    public EnemyState currentState = EnemyState.Patrol;
    private List<Node> path;
    private int currentWaypoint = 0;
    private AStar aStar;
    private List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPatrolPoint = 0;

    void Start()
    {
        aStar = FindObjectOfType<AStar>();
        GeneratePatrolPoints();
    }

    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol(distanceToTarget);
                break;
            case EnemyState.Alert:
                Alert(distanceToTarget);
                break;
            case EnemyState.Chase:
                Chase(distanceToTarget);
                break;
        }
    }

    void Patrol(float distanceToTarget)
    {
        if (distanceToTarget <= alertRadius)
        {
            currentState = EnemyState.Alert;
            return;
        }

        if (patrolPoints.Count == 0)
        {
            GeneratePatrolPoints();
            return;
        }

        if (currentPatrolPoint >= patrolPoints.Count)
        {
            currentPatrolPoint = 0;
        }

        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPatrolPoint], speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint]) < nextWaypointDistance)
        {
            currentPatrolPoint++;
        }
    }

    void Alert(float distanceToTarget)
    {
        if (distanceToTarget <= chaseRadius)
        {
            currentState = EnemyState.Chase;
            UpdatePath();
            return;
        }

        if (distanceToTarget > alertRadius)
        {
            currentState = EnemyState.Patrol;
        }
    }

    void Chase(float distanceToTarget)
    {
        if (distanceToTarget > chaseRadius)
        {
            currentState = EnemyState.Alert;
            return;
        }

        if (path == null || path.Count == 0)
        {
            return;
        }

        if (currentWaypoint >= nextWaypointDistance)
        {
            UpdatePath();
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

    void GeneratePatrolPoints()
    {
        patrolPoints.Clear();
        for (int i = 0; i < 4; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;
            randomDirection.y = transform.position.y;
            patrolPoints.Add(randomDirection);
        }
    }
}