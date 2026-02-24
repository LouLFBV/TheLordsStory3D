//using UnityEngine;

//public class MoveState : PlayerState
//{
//    public MoveState(PlayerController player) : base(player) { }

//    public override void Update()
//    {
//        if (player.Input.MoveInput == Vector2.zero)
//            player.StateMachine.ChangeState(player.IdleState);

//        if (player.Input.AttackPressed)
//            player.StateMachine.ChangeState(player.AttackState);

//        if (player.Input.RollPressed && player.Stamina.CanSpend(20))
//            player.StateMachine.ChangeState(player.RollState);
//    }

//    public override void FixedUpdate()
//    {
//        player.Motor.Move(player.Input.MoveInput);
//    }
//}

using UnityEngine;

public class MoveState : PlayerState
{
    private int hHash = Animator.StringToHash("H");
    private int vHash = Animator.StringToHash("V");
    private int speedHash = Animator.StringToHash("Speed");
    private Vector2 cachedInput;

    public MoveState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.Animator.applyRootMotion = true;
    }


    public override void Update()
    {
        cachedInput = player.Input.MoveInput;

        if (cachedInput == Vector2.zero)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
            return;
        }

        player.Animator.SetFloat(hHash, cachedInput.x, 0.1f, Time.deltaTime);
        player.Animator.SetFloat(vHash, cachedInput.y, 0.1f, Time.deltaTime);

        float magnitude = Mathf.Clamp01(cachedInput.magnitude);
        player.Animator.SetFloat(speedHash, magnitude, 0.1f, Time.deltaTime);
    }

    public override void FixedUpdate()
    {
        player.Motor.RotateTowardsInput(cachedInput);
    }
}