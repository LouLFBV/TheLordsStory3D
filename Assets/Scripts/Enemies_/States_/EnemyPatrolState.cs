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
        isWaiting = false; // Reset important
        FindNewDestination();
    }

    public override void Update()
    {
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

        // Sécurité : On ne vérifie la distance QUE si l'agent a un chemin et qu'il est en mouvement
        if (agent.hasPath && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                StartWaiting();
            }
        }

        // Animation plus fluide
        enemy.Animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
    }

    private void FindNewDestination()
    {
        // On prend un point aléatoire
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;

        // FORCE une distance minimum (ex: au moins 4 mètres)
        if (randomDirection.magnitude < 4f)
        {
            randomDirection = randomDirection.normalized * 4f;
        }

        randomDirection += enemy.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            agent.isStopped = false;
            // On s'assure que la flèche bleue (vélocité) se réactive
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