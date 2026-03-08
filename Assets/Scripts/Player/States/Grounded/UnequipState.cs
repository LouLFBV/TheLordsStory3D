using UnityEngine;

public class UnequipState : GroundedState
{
    private float timer;
    private float duration = 0.5f; // Souvent plus court que l'équipement

    public UnequipState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        player.Rigidbody.linearVelocity = Vector3.zero;

        HandWeapon type = player.PendingUnequipType;

        // On déclenche l'animation de rangement
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

    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            // Une fois rangé, on repasse en Idle (ou Move)
            player.StateMachine.ChangeState(player.Input.MoveInput != Vector2.zero
                ? PlayerStateType.Move : PlayerStateType.Idle);
        }
    }

    public void HandleWeaponRemoval()
    {
        if (player.PendingLibraryItem != null)
        {
            // Désactiver le nouveau prefab
            player.PendingLibraryItem.itemPrefab.SetActive(false);

            // Activer les éléments visuels inutiles (ex: carquois si arc, etc.)
            foreach (var element in player.PendingLibraryItem.elementsToDisable)
            {
                element.SetActive(true);
            }
        }
    }
}