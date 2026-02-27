using UnityEngine;

public class HitState : PlayerState
{
    public HitState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // 1. On joue le trigger de l'anim
        player.Animator.SetTrigger("Hit");

        // 2. On s'assure de couper le mouvement (Root motion peut être utile ou non selon ton besoin)
        player.Animator.applyRootMotion = true;
    }

    public override void Update()
    {
        // On surveille l'animation "Hit" sur le layer 0
        AnimatorStateInfo stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);

        // Si l'animation est terminée (95% ou plus)
        if (stateInfo.IsName("Hit") && stateInfo.normalizedTime >= 0.95f)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        // Reset le trigger pour éviter qu'il ne se relance accidentellement
        player.Animator.ResetTrigger("Hit");
    }
}