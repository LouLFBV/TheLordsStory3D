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
        {
            if (player.usingSpecialAttack)
            {
                player.CurrentAttack = combatData?.specialAttack;
                Debug.Log($"Starting Special Attack: {player.CurrentAttack}");
            }
            else
            {
                player.CurrentAttack = combatData?.startingAttack;
                Debug.Log($"Starting Attack: {player.CurrentAttack}");
            }
        }
        if (player.CurrentAttack == null)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
            return;
        }

        // On ordonne l'exécution
        player.Combat.ExecuteAttack(player.CurrentAttack);
        player.Animator.SetLayerWeight(player.Combat.attackLayer, 1f);
    }

    public override void Update()
    {
        // 1. Buffer d'input : si on clique pendant que canCombo est vrai
        if (player.Input.AttackSpecialPressed && player.Combat.CanComboNext())
        {
            Debug.Log("Input d'attaque enregistré pour le combo !");
            player.Input.UseAttackInput();
            player.Input.UseAttackSpecialInput();
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
        player.usingSpecialAttack = false;
        //player.Animator.applyRootMotion = false;

        player.Animator.SetLayerWeight(player.Combat.attackLayer, 0f);
    }

    public void OnAnimationFinished() => animationFinished = true;
}