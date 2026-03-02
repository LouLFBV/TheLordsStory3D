using UnityEngine;
public class EquipState : GroundedState
{
    private float timer;
    private float duration = 0.8f; // Ajuste selon la longueur de tes anims

    public EquipState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        player.Rigidbody.linearVelocity = Vector3.zero;

        // On déclenche l'animation selon le type stocké
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

    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            player.StateMachine.ChangeState(player.Input.MoveInput != Vector2.zero
                ? PlayerStateType.Move : PlayerStateType.Idle);
        }
    }
}