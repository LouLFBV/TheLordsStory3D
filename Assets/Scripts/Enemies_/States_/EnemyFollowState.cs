using UnityEngine;

public class EnemyFollowState : EnemyState
{
    public EnemyFollowState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        // On peut déclencher une animation de course ici
        // enemy.Animator.CrossFade("Run", 0.1f);

        agent.isStopped = false;
        // On remet la vitesse de poursuite
        agent.speed = 4f; // Tu pourras plus tard tirer ça d'un SO
    }

    public override void Update()
    {
        if (enemy.Target == null)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.Target.position);

        // 1. Si on est assez proche pour attaquer
        if (distance <= enemy.AttackRadius)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Attack);
            return;
        }

        // 2. Si le joueur s'est trop éloigné (on l'a perdu)
        if (distance > enemy.DetectionRadius * 1.5f)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);
            return;
        }

        // 3. Sinon, on met à jour la destination
        agent.SetDestination(enemy.Target.position);

        // Animation : on envoie la vitesse au paramètre "Speed" de l'animator
        enemy.Animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
    }

    public override void Exit()
    {
        // On arrête l'agent proprement en quittant l'état
        if (agent.isActiveAndEnabled)
            agent.isStopped = true;
    }
}