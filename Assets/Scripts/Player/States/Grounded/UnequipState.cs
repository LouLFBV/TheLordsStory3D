using UnityEngine;

public class UnequipState : GroundedState
{
    public UnequipState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.Rigidbody.linearVelocity = Vector3.zero;


        if (player.PendingWeaponItem.itemType == ItemType.Consumable)
        {
            player.Animator.SetBool("CarryingConsumable", false);
            return;
        }

        // On déclenche l'animation de rangement
        PlayUnequipAnimation(player.PendingUnequipType);
    }

    private void PlayUnequipAnimation(HandWeapon type)
    {
        switch (type)
        {
            case HandWeapon.Bow:
                player.Animator.SetTrigger("DesequipBow");
                player.Animator.SetBool("BowEquipped", false);
                break;
            case HandWeapon.TwoHanded:
                player.Animator.SetTrigger("DesequipLongSword");
                player.Animator.SetBool("IsTwoHandedWeapon", false);
                break;
            case HandWeapon.OneHanded:
                player.Animator.SetTrigger("DesequipSword");
                player.Animator.SetBool("IsOneHandedWeapon", false);
                break;
        }
    }
    public void HandleWeaponRemoval()
    {
        if (player.PendingLibraryItem != null)
        {
            // Désactiver le nouveau prefab;
            player.PendingLibraryItem.itemPrefab.SetActive(false);

            // Activer les éléments visuels inutiles (ex: carquois si arc, etc.)
            foreach (var element in player.PendingLibraryItem.elementsToDisable)
            {
                element.SetActive(true);
            }
            player.StateMachine.ChangeState(player.Input.MoveInput != Vector2.zero
                ? PlayerStateType.Move : PlayerStateType.Idle);
        }
        else
            Debug.LogWarning("PendingLibraryItem is null in HandleWeaponRemoval, cannot disable prefab or enable elements.");
    }
}