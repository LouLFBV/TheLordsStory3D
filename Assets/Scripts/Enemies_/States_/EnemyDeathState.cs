using UnityEngine;
public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        // 1. On arrête les mouvements
        agent.isStopped = true;
        agent.enabled = false; // Pour ne plus qu'il pousse les autres

        // 2. On lance l'animation de mort
        enemy.Animator.SetTrigger("Die");

        // 3. On désactive les collisions pour ne pas gêner le joueur
        if (enemy.GetComponent<Collider>())
            enemy.GetComponent<Collider>().enabled = false;

        // 4. On cache l'UI de vie
        enemy.SetLockOnIndicator(false);

        Debug.Log($"{enemy.gameObject.name} est mort.");
    }

    public override void Update() { }
    public override void Exit() { }
}