using UnityEngine;

public class RollState : PlayerGroundedState
{
	private float rollDuration = 0.8f;
	private float iframeStart = 0.15f;
	private float iframeEnd = 0.45f;

	private float timer;

	public RollState(PlayerController player) : base(player) { }

	//public override void Enter()
	//{
	//	player.Stamina.Spend(20);

	//	player.Animator.applyRootMotion = true;
	//	player.Animator.SetTrigger("Roll");

	//	player.Motor.EnableRollCollider(true);

	//	timer = 0f;
	//}

	//public override void Update()
	//{
	//	timer += Time.deltaTime;

	//	if (timer >= iframeStart && timer <= iframeEnd)
	//		player.SetInvincibility(true);
	//	else
	//		player.SetInvincibility(false);

	//	if (timer >= rollDuration)
	//		player.StateMachine.ChangeState(player.IdleState);

	//	if (player.Input.AttackPressed)
	//	{
	//		player.BufferAction(() =>
	//			player.StateMachine.ChangeState(player.AttackState));
	//	}
	//}

	//public override void Exit()
	//{
	//	player.SetInvincibility(false);
	//	player.Animator.applyRootMotion = false;
	//	player.Motor.EnableRollCollider(false);
	//	player.ExecuteBufferedAction();
	//}
}