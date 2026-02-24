//using UnityEngine;

//public class HitState : PlayerState
//{
//    private float hitDuration = 0.5f;
//    private float timer;

//    public HitState(PlayerController player) : base(player) { }

//    public override void Enter()
//    {
//        timer = 0f;
//        player.Animator.applyRootMotion = true;
//        player.Animator.SetTrigger("Hit");
//    }

//    public override void Update()
//    {
//        timer += Time.deltaTime;

//        if (timer >= hitDuration)
//            player.StateMachine.ChangeState(player.IdleState);
//    }

//    public override void Exit()
//    {
//        player.Animator.applyRootMotion = false;
//    }
//}