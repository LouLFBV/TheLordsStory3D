//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    public PlayerStateMachine StateMachine { get; private set; }
//    public PlayerInputHandler Input { get; private set; }
//    public CharacterMotor Motor { get; private set; }
//    public CombatSystem Combat { get; private set; }
//    public StaminaSystem Stamina { get; private set; }
//    public Animator Animator { get; private set; }
//    public IdleState IdleState { get; private set; }
//    public MoveState MoveState { get; private set; }
//    public AttackState AttackState { get; private set; }
//    public RollState RollState { get; private set; }
//    public HitState HitState { get; private set; }
//    public bool IsInvincible { get; private set; }


//    public float inputBufferTime = 0.2f;
//    private float bufferTimer;
//    private System.Action bufferedAction;

//    private void Awake()
//    {
//        var allStates = new Dictionary<PlayerStateType, PlayerState>
//    {
//        { PlayerStateType.Idle, new IdleState(this) },
//        { PlayerStateType.Move, new MoveState(this) },
//        { PlayerStateType.Attack, new AttackState(this) },
//        { PlayerStateType.Roll, new RollState(this) },
//        { PlayerStateType.Hit, new HitState(this) },
//    };

//        StateMachine = new PlayerStateMachine(allStates);
//    }

//    private void Start()
//    {
//        StateMachine.Initialize(PlayerStateType.Idle);
//    }

//    private void Update()
//    {
//        StateMachine.Update();

//        if (bufferTimer > 0)
//            bufferTimer -= Time.deltaTime;
//        else
//            bufferedAction = null;
//    }

//    private void FixedUpdate()
//    {
//        StateMachine.FixedUpdate();
//    }

//    public void BufferAction(System.Action action)
//    {
//        bufferedAction = action;
//        bufferTimer = inputBufferTime;
//    }


//    public void ExecuteBufferedAction()
//    {
//        bufferedAction?.Invoke();
//        bufferedAction = null;
//    }


//    public void SetInvincibility(bool value)
//    {
//        IsInvincible = value;
//    }
//}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputHandler Input { get; private set; }
    public CharacterMotor Motor { get; private set; }
    public Animator Animator { get; private set; }

    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }

    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        Input = GetComponent<PlayerInputHandler>();
        Motor = GetComponent<CharacterMotor>();
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>(); // Ensure Rigidbody is assigned here

        IdleState = new IdleState(this);
        MoveState = new MoveState(this);

        StateMachine = new PlayerStateMachine(
            new System.Collections.Generic.Dictionary<PlayerStateType, PlayerState>
            {
                { PlayerStateType.Idle, IdleState },
                { PlayerStateType.Move, MoveState }
            }
        );
    }

    private void Start()
    {
        StateMachine.Initialize(PlayerStateType.Idle);
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }


    private void OnAnimatorMove()
    {
        if (!Animator.applyRootMotion)
            return;

        Rigidbody.MovePosition(
            Rigidbody.position + Animator.deltaPosition
        );

        Rigidbody.MoveRotation(
            Rigidbody.rotation * Animator.deltaRotation
        );
    }
}