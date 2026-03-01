using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputHandler Input { get; private set; }
    public CharacterMotor Motor { get; private set; }
    public HealthSystem Health { get; private set; }
    public Animator Animator { get; private set; }
    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public CombatSystem Combat { get; private set; }
    public StaminaSystem Stamina { get; private set; }
    public AttackState AttackState { get; private set; }
    public RollState RollState { get; private set; }
    public HitState HitState { get; private set; }
    public JumpState JumpState { get; private set; }
    public FallState FallState { get; private set; }
    public AimState AimState { get; private set; }
    public DeathState DeathState { get; private set; }
    public PaletteSystem PaletteSystem { get; private set; }
    [SerializeField] private PaletteSystem paletteSystem;
    public Rigidbody Rigidbody { get; private set; }
    public ThirdPersonCameraController Camera { get; private set; }
    [SerializeField] private ThirdPersonCameraController cameraScript;
    
    [Header("Combat Settings")]
    public AttackSO defaultLightAttack;

    private void Awake()
    {
        Input = GetComponent<PlayerInputHandler>();
        Motor = GetComponent<CharacterMotor>();
        Health = GetComponent<HealthSystem>();
        Stamina = GetComponent<StaminaSystem>();
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>(); // Ensure Rigidbody is assigned here
        Camera = cameraScript; // Assign the camera script reference
        PaletteSystem = paletteSystem; // Assign the palette system reference
        IdleState = new IdleState(this);
        MoveState = new MoveState(this);
        AttackState = new AttackState(this);
        RollState = new RollState(this);
        HitState = new HitState(this); 
        FallState = new FallState(this);
        JumpState = new JumpState(this);
        DeathState = new DeathState(this);
        AimState = new AimState(this);

        StateMachine = new PlayerStateMachine(
            new System.Collections.Generic.Dictionary<PlayerStateType, PlayerState>
            {
                { PlayerStateType.Idle, IdleState },
                { PlayerStateType.Move, MoveState },
                { PlayerStateType.Attack, AttackState},
                { PlayerStateType.Roll, RollState },
                { PlayerStateType.Hit, HitState },
                { PlayerStateType.Jump, JumpState },
                { PlayerStateType.Fall, FallState},
                { PlayerStateType.Death, DeathState },
                { PlayerStateType.Aim, AimState }
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
        PaletteSystem.HandlePaletteLogic(this);
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