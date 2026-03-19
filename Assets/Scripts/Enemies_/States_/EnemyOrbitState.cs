using UnityEngine;

public class EnemyOrbitState : EnemyState
{
    private float _directionTimer;
    private int _orbitDirection = 1; // 1 pour droite, -1 pour gauche
    private float _attackTimer = 3f;

    public EnemyOrbitState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Animator.SetBool("isStrafing", true);
        agent.stoppingDistance = enemy.AIManager.OrbitDistance;
        _directionTimer = Random.Range(2f, 4f);
    }

    public override void Update()
    {
        if (enemy.Target == null) return;

        float distance = Vector3.Distance(enemy.transform.position, enemy.Target.position);

        // 1. Si le joueur s'éloigne trop, on repasse en Follow
        if (distance > enemy.DetectionRadius)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Follow);
            return;
        }

        // 2. Si on est à portée d'attaque, on a une chance de frapper
        if (distance <= enemy.AttackRadius)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Attack);
            return;
        }

        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Follow);
            return;
        }

        // 3. Logique de déplacement orbital (Strafing)
        OrbitMovement();

        // Update Animation
        enemy.Animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed, 0.1f, Time.deltaTime);
    }

    private void OrbitMovement()
    {
        // On change de direction de temps en temps
        _directionTimer -= Time.deltaTime;
        if (_directionTimer <= 0)
        {
            _orbitDirection *= -1;
            _directionTimer = Random.Range(2f, 5f);
        }

        // Calcul du point autour du joueur
        Vector3 directionToPlayer = (enemy.transform.position - enemy.Target.position).normalized;
        Vector3 orbitOffset = Quaternion.AngleAxis(_orbitDirection * 20f, Vector3.up) * directionToPlayer;
        Vector3 targetPos = enemy.Target.position + (orbitOffset * enemy.AIManager.OrbitDistance);

        agent.SetDestination(targetPos);

        // On force l'ours à toujours regarder le joueur
        Vector3 lookDir = (enemy.Target.position - enemy.transform.position).normalized;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
        }
    }
    public override void Exit()
    {
        enemy.Animator.SetBool("isStrafing", false);
    }
}