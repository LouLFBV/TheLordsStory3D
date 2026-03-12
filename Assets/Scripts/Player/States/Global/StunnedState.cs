using UnityEngine;

public class StunnedState : PlayerState
{
    private float stunTimer;
    private float maxStunDuration = 3.0f; // Durée de l'étourdissement

    public StunnedState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        stunTimer = 0f;

        // 1. On coupe TOUT
        player.Animator.applyRootMotion = true;
        player.Combat.AE_HitboxClose();
        player.CurrentAttack = null;

        // 2. On joue l'animation de Stun (boucle ou longue anim)
        player.Animator.SetBool("IsStunned", true);

        // 3. Optionnel : Petit effet visuel (étoiles au-dessus de la tête)
        // player.Effects.PlayStunVFX(true);

        Debug.Log("ÉTAT ÉTOURDI : Défense brisée !");
    }

    public override void Update()
    {
        stunTimer += Time.deltaTime;

        // On reste dans cet état tant que le timer n'est pas fini
        if (stunTimer >= maxStunDuration)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // On arrête l'animation et on rend le contrôle
        player.Animator.SetBool("IsStunned", false);

        // Reset du Poise pour éviter de se faire re-stun immédiatement
        player.Poise.ResetPoise();

        Debug.Log("Fin de l'étourdissement.");
    }
}