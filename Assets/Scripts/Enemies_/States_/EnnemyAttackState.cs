using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private bool isAnimationFinished;
    private float _exitTimer;
    private float _currentPostAttackDelay; // Délai dynamique récupéré de l'AttackSO
    private AttackSO _currentAttack;

    public EnemyAttackState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        isAnimationFinished = false;
        _exitTimer = 0f;
        _currentAttack = null;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        FaceTarget();

        // 1. On cherche l'attaque qui remplit les conditions (Range + Cooldown)
        _currentAttack = enemy.GetBestAttack();

        if (_currentAttack != null)
        {
            // 2. On récupère le délai spécifique à cette attaque
            _currentPostAttackDelay = _currentAttack.postAttackDelay;

            // 3. On prépare l'arme/hitbox et on lance l'anim
            enemy.PrepareAttack(_currentAttack);
            enemy.Combat.ExecuteAttack(_currentAttack);
        }
        else
        {
            // Sécurité : Si aucune attaque n'est prête, on termine immédiatement
            _currentPostAttackDelay = 0f;
            isAnimationFinished = true;
        }
    }

    public override void Update()
    {
        // On continue de pivoter tant que l'anim n'est pas finie 
        // (ou tu peux stopper la rotation via un Event si besoin)
        if (!isAnimationFinished)
        {
            FaceTarget();
        }
        else
        {
            // 4. Une fois l'animation finie, on attend le délai de l'AttackSO
            _exitTimer += Time.deltaTime;
            if (_exitTimer >= _currentPostAttackDelay)
            {
                DetermineNextState();
            }
        }
    }

    private void DetermineNextState()
    {
        // On cherche si une attaque est DISPONIBLE (sans AttackRadius)
        AttackSO nextPotentialAttack = enemy.PeekBestAttack(); // Une méthode qui check sans modifier le temps

        if (nextPotentialAttack != null)
        {
            // On se relance directement pour l'attaque suivante
            enemy.StateMachine.ChangeState(EnemyStateType.Attack);
        }
        else
        {
            // Aucune attaque n'est possible (portée ou cooldown)
            // Si on est très près, on orbite, sinon on suit
            float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);

            // Utilise une valeur de sécurité (ex: 3m) ou la range max de ton attaque la plus courte
            if (distance <= 3f)
                enemy.StateMachine.ChangeState(EnemyStateType.Orbit);
            else
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
            // Vitesse de rotation pendant l'attaque (peut être ajustée par SO aussi !)
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void OnAnimationFinished()
    {
        isAnimationFinished = true;
    }

    public override void Exit()
    {
        // Reset de la vitesse pour le prochain état
        agent.isStopped = false;

        // Cooldown global pour éviter l'orbite spam
        enemy.AIManager.StartOrbitCooldown(3f);
    }
}