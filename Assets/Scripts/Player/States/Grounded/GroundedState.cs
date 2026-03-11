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
            //player.Stamina.Spend(10f);
            player.StateMachine.ChangeState(PlayerStateType.Jump);
            return;
        }

        // 3. PRIORITÉ : L'Attaque ou l'Arc
        if (player.Input.AttackPressed && player.Stamina.HasStamina())
        {
            HandleAttackInput();
            return;
        }

        // 4. PRIORITÉ : La Visée (AimState)
        // Si on n'attaque pas, mais qu'on maintient le bouton de visée
        if (player.Input.AimHeld)
        {
            player.StateMachine.ChangeState(PlayerStateType.Aim);
            return;
        }

        // 5. PRIORITÉ : La Roulade
        if (player.Input.RollPressed && player.Stamina.HasStamina())
        {
            player.StateMachine.ChangeState(PlayerStateType.Roll);
            return;
        }
    }

    private void HandleAttackInput()
    {
        // On récupère l'arme active via ton PaletteSystem
        ItemData activeWeapon = PaletteSystem.instance.slotManager.weaponSlots[0].isEquipped ?
                                PaletteSystem.instance.slotManager.weaponSlots[0].slotItemData :
                                PaletteSystem.instance.slotManager.weaponSlots[1].slotItemData;

        if (activeWeapon == null) return;

        // --- DISTINCTION ARC / MÊLÉE ---
        if (activeWeapon.handWeaponType == HandWeapon.Bow)
        {
            // On vérifie les munitions via ton BowBehaviour
            if (player.GetComponent<BowBehaviour>().VerifIfCanShoot())
            {
                Debug.Log("Passage en BowChargeState depuis GroundedState");
                player.StateMachine.ChangeState(PlayerStateType.BowCharge);
            }
        }
        else
        {
            player.StateMachine.ChangeState(PlayerStateType.Attack);
        }
    }
}