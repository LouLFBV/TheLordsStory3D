using UnityEngine;

public class BowChargeState : GroundedState
{
    private float chargeTime = 1.5f; // Temps nécessaire pour charger complètement
    private float timer;
    public BowChargeState(PlayerController player) : base(player) { }
    public override void Enter()
    {
        base.Enter();
        timer = 0f;
        player.Animator.SetTrigger("BowCharge");
        player.Animator.applyRootMotion = true;
    }
    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= chargeTime)
        {
            // Charge complète, on peut tirer
            player.StateMachine.ChangeState(PlayerStateType.Aim);
            return;
        }
        if (!player.Input.AttackPressed)
        {
            // Si le joueur relâche avant la charge complète, on annule
            player.StateMachine.ChangeState(PlayerStateType.Idle);
            return;
        }
    }
    public override void Exit()
    {
        base.Exit();
        player.Animator.applyRootMotion = false;
    }
}