using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private bool isAnimationFinished;
    private float _exitTimer;
    private const float POST_ATTACK_DELAY = 0.5f; // Petit délai de sécurité après l'animation

    public EnemyAttackState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        isAnimationFinished = false;
        _exitTimer = 0f;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        FaceTarget();

        // On utilise ta nouvelle méthode intelligente de sélection
        AttackSO attackToExecute = enemy.GetBestAttack();

        if (attackToExecute != null)
        {
            enemy.PrepareAttack(attackToExecute);
            enemy.Combat.ExecuteAttack(attackToExecute);
        }
        else
        {
            // Si aucune attaque n'est valide (cooldowns ou distance), on quitte
            isAnimationFinished = true;
        }
    }

    public override void Update()
    {
        // On continue de pivoter tant que l'attaque n'est pas "lancée" physiquement
        // (Certaines attaques de boss demandent de ne plus pivoter à mi-chemin)
        if (!isAnimationFinished)
        {
            FaceTarget();
        }
        else
        {
            // Une fois l'animation finie, on attend un tout petit peu
            // pour éviter les transitions trop brusques
            _exitTimer += Time.deltaTime;
            if (_exitTimer >= POST_ATTACK_DELAY)
            {
                DetermineNextState();
            }
        }
    }

    private void DetermineNextState()
    {
        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);

        // Au lieu de boucler directement (Enter()), on repasse par Follow 
        // ou Orbit pour laisser le Cooldown de l'AttackSO respirer.
        if (distance <= enemy.AttackRadius)
        {
            // Si on est encore au corps à corps, on peut tenter une autre attaque
            // mais on repasse par la StateMachine pour être propre
            AttackSO nextAttack = enemy.GetBestAttack();

            if (nextAttack != null)
            {
                enemy.StateMachine.ChangeState(EnemyStateType.Attack);
            }
            else
            {
                // Si aucune attaque n'est prête (cooldown), on recule ou on observe
                enemy.StateMachine.ChangeState(EnemyStateType.Orbit);
            }
        }
        else
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Follow);
        }
    }

    private void FaceTarget()
    {
        if (enemy.target == null) return;

        Vector3 direction = (enemy.target.position - enemy.transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void OnAnimationFinished()
    {
        isAnimationFinished = true;
    }

    public override void Exit()
    {
        // On lance le cooldown d'orbite global pour que l'IA repasse en "Follow" agressif
        // si elle n'a plus d'attaques dispos en orbite.
        enemy.AIManager.StartOrbitCooldown(3f);
    }
}