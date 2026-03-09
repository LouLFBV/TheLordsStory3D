using UnityEngine;
public class EquipState : GroundedState
{

    public EquipState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.Rigidbody.linearVelocity = Vector3.zero;

        if (player.PendingWeaponItem.itemType == ItemType.Consumable)
        {
            player.Animator.SetBool("CarryingConsumable", true);
            return;
        }
        PlayEquipAnimation(player.PendingWeaponType);
    }

    private void PlayEquipAnimation(HandWeapon type)
    {
        switch (type)
        {
            case HandWeapon.Bow:
                player.Animator.SetTrigger("EquipBow");
                player.Animator.SetBool("BowEquipped", true);
                break;
            case HandWeapon.TwoHanded:
                player.Animator.SetTrigger("EquipLongSword");
                break;
            case HandWeapon.OneHanded:
                player.Animator.SetTrigger("EquipSword");
                break;
        }
    }

    public void HandleWeaponSwitch()
    {
        if (player.PendingLibraryItem != null)
        {
            // Activer le nouveau prefab
            player.PendingLibraryItem.itemPrefab.SetActive(true);
            // Désactiver les éléments visuels inutiles (ex: carquois si arc, etc.)
            foreach (var element in player.PendingLibraryItem.elementsToDisable)
            {
                element.SetActive(false);
            }
            player.StateMachine.ChangeState(player.Input.MoveInput != Vector2.zero
                ? PlayerStateType.Move : PlayerStateType.Idle);
        }
    }
}