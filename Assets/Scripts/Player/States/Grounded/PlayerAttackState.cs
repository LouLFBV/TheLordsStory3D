using UnityEngine;

public class PlayerAttackState : PlayerGroundedState
{
    private bool animationFinished; 

    public PlayerAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        animationFinished = false;

        // On récupère la première attaque de l'arme
        var combatData = player.PendingWeaponItem?.combatData;
        if (player.CurrentAttack == null)
            player.CurrentAttack = combatData?.startingAttack;

        // On ordonne l'exécution
        player.Combat.ExecuteAttack(player.CurrentAttack);
        player.Animator.SetLayerWeight(player.Combat.attackLayer, 1f);
    }

    public override void Update()
    {
        // 1. Buffer d'input : si on clique pendant que canCombo est vrai
        if (player.Input.AttackPressed && player.Combat.CanComboNext())
        {
            player.Input.UseAttackInput();
            if (player.CurrentAttack.nextAttack != null)
            {
                // On prépare la suite
                player.CurrentAttack = player.CurrentAttack.nextAttack;
                player.Combat.ExecuteAttack(player.CurrentAttack);
            }
        }

        if (animationFinished)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.CurrentAttack = null; // Reset le combo
        //player.Animator.applyRootMotion = false;

        player.Animator.SetLayerWeight(player.Combat.attackLayer, 0f);
    }

    public void OnAnimationFinished() => animationFinished = true;
}