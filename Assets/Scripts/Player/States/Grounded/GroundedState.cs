//using UnityEngine;

//public class GroundedState : PlayerState
//{
//    public AttackSO lightAttack1;
//    public GroundedState(PlayerController player) : base(player) { }
//    public override void Enter()
//    {
//        // On force l'Animator à revenir sur l'Idle du Layer 0 
//        // et on fait un fondu pour effacer l'attaque du Layer 10
//        player.Animator.CrossFadeInFixedTime("Idle", 0.2f, 0);

//        // Si tu utilises un Layer 10 avec du poids (Weight), 
//        // il faut parfois "vider" le layer 10 pour qu'il ne fige pas le perso
//        player.Animator.SetLayerWeight(10, 0f);
//    }
//    public override void Update()
//    {// Si on perd le sol, on force le passage en FallState
//        if (!player.Motor.IsGrounded())
//        {
//            player.StateMachine.ChangeState(PlayerStateType.Fall); // à créer
//            return;
//        }
//        if (player.Input.AttackPressed && player.Stamina.HasStamina())
//        {
//            if (player.StateMachine.CurrentState.GetType() != typeof(AttackState))
//            {
//                var attackState = (AttackState)player.StateMachine.GetState(PlayerStateType.Attack);
//                attackState.SetAttack(player.defaultLightAttack);
//                player.StateMachine.ChangeState(PlayerStateType.Attack);
//            }
//        }

//        if (player.Input.JumpPressed && player.Stamina.HasStamina())
//        {
//            // On peut même consommer un peu de stamina pour le saut !
//            player.Stamina.Spend(10f);
//            player.StateMachine.ChangeState(PlayerStateType.Jump);
//        }
//    }
//}

using UnityEngine;

public class GroundedState : PlayerState
{
    public GroundedState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.Animator.applyRootMotion = true; // Le sol reprend le contrôle via l'anim
    
        // On ne nettoie les layers que si on vient d'un état "non-grounded" 
        // ou d'une attaque, pour éviter les saccades entre Idle et Move.
    }

    public override void Update()
    {
        base.Update();

        // 1. PRIORITÉ : La Chute
        if (!player.Motor.IsGrounded())
        {
            player.StateMachine.ChangeState(PlayerStateType.Fall);
            return;
        }

        // 2. PRIORITÉ : Le Saut
        if (player.Input.JumpPressed && player.Stamina.HasStamina())
        {
            // On consomme la stamina ici
            player.Stamina.Spend(10f);
            player.StateMachine.ChangeState(PlayerStateType.Jump);
            return;
        }

        // 3. PRIORITÉ : L'Attaque
        if (player.Input.AttackPressed && player.Stamina.HasStamina())
        {
            // On vérifie quelle arme la palette a activé
            ItemData activeWeapon = PaletteSystem.instance.weapon1Slot.isEquipped ?
                                     PaletteSystem.instance.weapon1Slot.slotItemData :
                                     PaletteSystem.instance.weapon1Slot.slotItemData;

            if (activeWeapon != null)
            {
                var attackState = (AttackState)player.StateMachine.GetState(PlayerStateType.Attack);
                // On récupère l'AttackSO liée à l'objet de la palette
                //attackState.SetAttack(activeWeapon.lightAttackSO);
                attackState.SetAttack(player.defaultLightAttack);
                player.StateMachine.ChangeState(PlayerStateType.Attack);
            }
            return;
        }
        // 4. PRIORITÉ : La Roulade (Si tu l'as déjà implémentée)
        if (player.Input.RollPressed && player.Stamina.HasStamina())
        {
            player.StateMachine.ChangeState(PlayerStateType.Roll);
            return;
        }
    }
}