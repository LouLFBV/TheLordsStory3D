using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputHandler Input { get; private set; }
    public CharacterMotor Motor { get; private set; }
    public Animator Animator { get; private set; }
    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public CombatSystem Combat { get; private set; }
    public StaminaSystem Stamina { get; private set; }
    public AttackState AttackState { get; private set; }
    public RollState RollState { get; private set; }
    public HitState HitState { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public ThirdPersonCameraController Camera { get; private set; }
    [SerializeField] private ThirdPersonCameraController cameraScript;
    
    [Header("Combat Settings")]
    public AttackSO defaultLightAttack;

    public float MaxStamina = 100f;
    public float CurrentStamina { get; private set; }
    public float StaminaConsumptionRate = 30f; // par seconde lors du sprint
    public float StaminaRecoveryRate = 20f;    // par seconde quand non sprint

    private void Awake()
    {
        Input = GetComponent<PlayerInputHandler>();
        Motor = GetComponent<CharacterMotor>();
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>(); // Ensure Rigidbody is assigned here
        Camera = cameraScript; // Assign the camera script reference
        IdleState = new IdleState(this);
        MoveState = new MoveState(this);
        AttackState = new AttackState(this);
        RollState = new RollState(this);
        HitState = new HitState(this);

        StateMachine = new PlayerStateMachine(
            new System.Collections.Generic.Dictionary<PlayerStateType, PlayerState>
            {
                { PlayerStateType.Idle, IdleState },
                { PlayerStateType.Move, MoveState },
                { PlayerStateType.Attack, AttackState},
                { PlayerStateType.Roll, RollState },
                { PlayerStateType.Hit, HitState }
            }
        );
        CurrentStamina = MaxStamina;
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

    public void ConsumeStamina(float amount)
    {
        CurrentStamina = Mathf.Max(CurrentStamina - amount, 0f);
    }

    public void RecoverStamina(float amount)
    {
        CurrentStamina = Mathf.Min(CurrentStamina + amount, MaxStamina);
    }

    public bool HasStamina() => CurrentStamina > 0f;
}