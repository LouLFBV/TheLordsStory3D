using UnityEngine;

public class HitState : PlayerState
{
    private bool isAnimationFinished;

    public HitState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        isAnimationFinished = false;

        // 1. Annulation immédiate des actions en cours
        player.Combat.AE_HitboxClose(); // On coupe l'épée si on était en train d'attaquer
        player.CurrentAttack = null;     // On reset le combo

        // 2. Visuel
        player.Animator.SetTrigger("Hit");

        Debug.Log("Le joueur est titubant (Stagger) !");
    }

    public override void Update()
    {
        // On attend l'Animation Event "OnHitAnimationEnd"
        if (isAnimationFinished)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.Animator.ResetTrigger("Hit");
    }

    // Appelé par un Animation Event à la fin du clip de dégâts
    public void OnHitAnimationEnd()
    {
        isAnimationFinished = true;
    }
}