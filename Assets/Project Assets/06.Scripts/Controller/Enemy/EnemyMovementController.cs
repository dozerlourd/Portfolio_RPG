using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Patrol,
    Alert,
    Chase,
    Attack,
    Damaged
}

public class EnemyMovementController : MonoBehaviour
{
    [SerializeField] EnemyMainController enemyMainController;

    public EnemyState currentState = EnemyState.Patrol;
    public Transform target;
    public float patrolRadius = 10f;
    public float alertRadius = 5f;
    public float chaseRadius = 3f;
    public float attackRadius = 2.5f;
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float nextWaypointDistance = 1f;

    [SerializeField] float damageStunTime = 1;

    [Header("회전 기능 설정")]
    [SerializeField] private float rotationSpeed = 5f;
    private Vector3 previousPosition;

    private List<Node> path;
    private int currentWaypoint = 0;
    private AStar aStar;
    private List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPatrolPoint = 0;

    private float damageElapseTime = 0;
    

    Coroutine Co_moveRoutine;
    Coroutine Co_damageCheckRoutine;

    void Start()
    {
        aStar = FindObjectOfType<AStar>();
        GeneratePatrolPoints();
        previousPosition = transform.position;
        Co_moveRoutine = StartCoroutine(MoveStateRoutine());
    }

    void Update()
    {
        if (enemyMainController.IsDead)
        {
            if(Co_moveRoutine != null)
                StopCoroutine(Co_moveRoutine);
        }
        else
        {
            Vector3 movement = transform.position - previousPosition;
            if(movement.magnitude > 0.001f)
            {
                Vector3 moveDir = movement.normalized;
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            previousPosition = transform.position;
        }

    }

    IEnumerator MoveStateRoutine()
    {
        yield return new WaitUntil(() => enemyMainController.EnemyMovementController != null);

        while(true)
        {
            if(currentState == EnemyState.Damaged)
            {
                enemyMainController.SetAnimationIsEnd(1);
            }
            yield return new WaitUntil(() => !enemyMainController.IsAttacking && enemyMainController.AnimationIsEnd == 1);
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
                case EnemyState.Attack:
                    Attack(distanceToTarget);
                    break;
                case EnemyState.Damaged:
                    Co_damageCheckRoutine = StartCoroutine(CheckToDamagedEnd());
                    enemyMainController.SetIsAttacking(false);
                    yield return Co_damageCheckRoutine;
                    break;
            }
            yield return null;
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

        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPatrolPoint], patrolSpeed * Time.deltaTime);

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
        if (distanceToTarget <= attackRadius)
        {
            currentState = EnemyState.Attack;
            return;
        }

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

        transform.position = Vector3.MoveTowards(transform.position, path[currentWaypoint].worldPosition, chaseSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, path[currentWaypoint].worldPosition) < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void Attack(float distanceToTarget)
    {
        if (distanceToTarget > attackRadius)
        {
            currentState = EnemyState.Chase;
            return;
        }
    }

    IEnumerator CheckToDamagedEnd()
    {
        while(damageElapseTime < damageStunTime)
        {
            float deltaTime = Time.deltaTime;
            damageElapseTime += deltaTime;
            yield return new WaitForSeconds(deltaTime);
        }

        currentState = EnemyState.Patrol;
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

    public void SetEnemyState(EnemyState nextState) => currentState = nextState;
    public void InitDamageElapseTime() => damageElapseTime = 0;
}
