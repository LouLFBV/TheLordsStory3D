using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState
{
    private Vector3 destination;
    private float waitTimer;
    private bool isWaiting;

    // Paramètres (qu'on pourra sortir dans un SO plus tard)
    private float walkSpeed = 2f;
    private float patrolRadius = 8f;
    private float waitTimeMin = 2f;
    private float waitTimeMax = 5f;

    public EnemyPatrolState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        agent.speed = walkSpeed;
        agent.isStopped = false;
        FindNewDestination();
    }

    public override void Update()
    {
        // 1. Si on attend, on décrémente le timer
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                FindNewDestination();
            }
            return;
        }

        // 2. Si on est arrivé à destination, on commence à attendre
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartWaiting();
        }

        // 3. Animation : on met à jour le float "Speed"
        enemy.Animator.SetFloat("Speed", agent.velocity.magnitude / walkSpeed);
    }

    private void FindNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += enemy.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            destination = hit.position;
            agent.SetDestination(destination);
            agent.isStopped = false;
        }
    }

    private void StartWaiting()
    {
        isWaiting = true;
        waitTimer = Random.Range(waitTimeMin, waitTimeMax);
        agent.isStopped = true;
        enemy.Animator.SetFloat("Speed", 0f);
    }

    public override void Exit()
    {
        isWaiting = false;
        if (agent.isActiveAndEnabled)
            agent.isStopped = true;
    }
}