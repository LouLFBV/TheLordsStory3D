using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Core Components")]
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputHandler Input { get; private set; }
    public CharacterMotor Motor { get; private set; }
    public Animator Animator { get; private set; }

    [Header("States")]
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
    public EquipState EquipState { get; private set; }
    public UnequipState UnequipState { get; private set; }
    public CrouchState CrouchState { get; private set; }
    public DeathState DeathState { get; private set; }


    [Header("Systems")]
    public HealthSystem Health { get; private set; }
    public ArmorSystem Armor { get; private set; }
    public PaletteSystem Palette { get; private set; }
    [SerializeField] private PaletteSystem palette;

    public Rigidbody Rigidbody { get; private set; }
    public ThirdPersonCameraController Camera { get; private set; }
    [SerializeField] private ThirdPersonCameraController cameraScript;
    public ItemData PendingWeaponItem { get; set; }
    public HandWeapon PendingWeaponType { get; set; }
    public HandWeapon PendingUnequipType { get; set; }

    [Header("Combat Settings")]
    public AttackSO defaultLightAttack;

    private void Awake()
    {
        Input = GetComponent<PlayerInputHandler>();
        Motor = GetComponent<CharacterMotor>();
        Health = GetComponent<HealthSystem>();
        Armor = GetComponent<ArmorSystem>();
        Stamina = GetComponent<StaminaSystem>();
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>(); // Ensure Rigidbody is assigned here
        Camera = cameraScript; // Assign the camera script reference
        Palette = palette; // Assign the palette system reference
        IdleState = new IdleState(this);
        MoveState = new MoveState(this);
        AttackState = new AttackState(this);
        RollState = new RollState(this);
        HitState = new HitState(this); 
        FallState = new FallState(this);
        JumpState = new JumpState(this);
        CrouchState = new CrouchState(this);
        DeathState = new DeathState(this);
        AimState = new AimState(this);
        EquipState = new EquipState(this);
        UnequipState = new UnequipState(this);

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
                { PlayerStateType.Aim, AimState },
                { PlayerStateType.Equip, EquipState },
                { PlayerStateType.Unequip, UnequipState },
                { PlayerStateType.Crouch, CrouchState }
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
        Palette.HandlePaletteLogic(this);
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