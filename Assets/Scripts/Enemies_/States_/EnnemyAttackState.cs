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
        _currentAttack = enemy.GetBestAttack();

        if (_currentAttack != null)
        {
            Debug.Log($"<color=red>[ATTACK]</color> Lancement de : {_currentAttack.animationName}");
            _currentPostAttackDelay = _currentAttack.postAttackDelay;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            enemy.PrepareAttack(_currentAttack);
            enemy.Combat.ExecuteAttack(_currentAttack);
        }
        else
        {
            Debug.LogWarning("[ATTACK] Enter sans attaque valide, retour immédiat.");
            isAnimationFinished = true;
            _currentPostAttackDelay = 0f;
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
                isAnimationFinished = false; 
                DetermineNextState();
            }
        }
    }


    private void DetermineNextState()
    {
        AttackSO nextPotentialAttack = enemy.PeekBestAttack();

        if (nextPotentialAttack != null)
        {
            Debug.Log($"<color=orange>[COMBO]</color> Enchaînement vers : {nextPotentialAttack.animationName}");

            // CORRECTION : On réinitialise manuellement l'état d'attaque 
            // au lieu de juste appeler ChangeState qui peut être ignoré
            this.Enter();
        }
        else
        {
            float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);
            Debug.Log($"[ATTACK] Fin d'enchaînement. Distance: {distance:F2}. Go Orbit/Follow.");

            if (distance <= 4f && enemy.AIManager.HasPermission(EnemyStateType.Orbit)) // On augmente un peu la zone pour forcer l'orbite
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
        enemy.lastAttackExitTime = Time.time;
        // Cooldown global pour éviter l'orbite spam
        enemy.AIManager.StartOrbitCooldown(2f);
    }
}