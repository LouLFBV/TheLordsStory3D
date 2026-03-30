using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private bool isAnimationFinished;

    public EnemyAttackState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        isAnimationFinished = false;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        FaceTarget();

        AttackSO attackToExecute = enemy.GetRandomAttack();

        if (attackToExecute != null)
        {
            // On prépare tout via le dictionnaire (Arme + Detector)
            enemy.PrepareAttack(attackToExecute);

            // On lance l'attaque
            enemy.Combat.ExecuteAttack(attackToExecute);
        }
        else
        {
            isAnimationFinished = true;
        }
    }


    public override void Update()
    {
        // On peut continuer à pivoter légèrement vers le joueur 
        // durant le début de l'attaque pour éviter qu'il ne tape dans le vide
        if (!isAnimationFinished)
        {
            FaceTarget();
        }

        // 4. Si l'animation est finie (via l'Animation Event), on décide quoi faire
        if (isAnimationFinished)
        {
            float distance = Vector3.Distance(enemy.transform.position, enemy.Target.position);

            if (distance <= enemy.AttackRadius)
            {
                // On peut enchaîner une autre attaque ou attendre un peu
                // Pour l'instant, on reset juste pour boucler
                Enter();
            }
            else
            {
                enemy.StateMachine.ChangeState(EnemyStateType.Follow);
            }
        }
    }

    private void FaceTarget()
    {
        if (enemy.Target == null) return;

        Vector3 direction = (enemy.Target.position - enemy.transform.position).normalized;
        direction.y = 0; // On ne veut pas que l'ennemi penche en avant/arrière

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void OnAnimationFinished()
    {
        isAnimationFinished = true;
    }

    public override void Exit()
    {
        enemy.AIManager.StartOrbitCooldown();
    }
}