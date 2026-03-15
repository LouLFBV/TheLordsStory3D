using UnityEngine;

public class PlayerController : MonoBehaviour, ICombatant
{
    [Header("Core Components")]
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputHandler Input { get; private set; }
    public CharacterMotor Motor { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    [Header("States")]
    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public CombatSystem Combat { get; private set; }
    public StaminaSystem Stamina { get; private set; }
    public AttackState AttackState { get; private set; }
    public RollState RollState { get; private set; }
    public HitState HitState { get; private set; }
    public StunnedState StunnedState { get; private set; }
    public JumpState JumpState { get; private set; }
    public FallState FallState { get; private set; }
    public AimState AimState { get; private set; }
    public EquipState EquipState { get; private set; }
    public UnequipState UnequipState { get; private set; }
    public BowChargeState BowChargeState { get; private set; }
    public CrouchState CrouchState { get; private set; }
    public DeathState DeathState { get; private set; }
    public UIState UIState { get; private set; }


    [Header("Systems")]
    public HealthSystem Health { get; private set; }
    public ArmorSystem Armor { get; private set; }
    public PoiseSystem Poise { get; private set; }
    public DamageReceiver DmgReceiver { get; private set; }
    public BowBehaviour Bow { get; private set; }

    public LockOnSystem LockOn { get; private set; }


    [Header("Combat Settings")]
    public AttackSO CurrentAttack { get; set; }
    public ItemData PendingWeaponItem { get; set; }
    public HandWeapon PendingWeaponType { get; set; }
    public HandWeapon PendingUnequipType { get; set; }


    [Header("Library")]
    public EquipmentLibrary equipmentLibrary;
    public EquipmentLibraryItem PendingLibraryItem { get; private set; }

    [Header("Others")]
    public UIPanelType RequestedPanelType { get; set; }
    private UIPanelType? _previousPanelType = null;
    public ItemData ItemQueuedToEquip;

    private void Awake()
    {
        Input = GetComponent<PlayerInputHandler>();
        Motor = GetComponent<CharacterMotor>();
        Health = GetComponent<HealthSystem>();
        Armor = GetComponent<ArmorSystem>();
        Stamina = GetComponent<StaminaSystem>();
        Animator = GetComponent<Animator>();
        Combat = GetComponent<CombatSystem>();
        Rigidbody = GetComponent<Rigidbody>();
        DmgReceiver = GetComponent<DamageReceiver>();
        Poise = GetComponent<PoiseSystem>();
        Bow = GetComponent<BowBehaviour>();
        LockOn = GetComponent<LockOnSystem>();
        IdleState = new IdleState(this);
        MoveState = new MoveState(this);
        AttackState = new AttackState(this);
        RollState = new RollState(this);
        HitState = new HitState(this);
        StunnedState = new StunnedState(this);
        FallState = new FallState(this);
        JumpState = new JumpState(this);
        CrouchState = new CrouchState(this);
        DeathState = new DeathState(this);
        AimState = new AimState(this);
        EquipState = new EquipState(this);
        UnequipState = new UnequipState(this);
        BowChargeState = new BowChargeState(this);
        UIState = new UIState(this);

        StateMachine = new PlayerStateMachine(
            new System.Collections.Generic.Dictionary<PlayerStateType, PlayerState>
            {
                { PlayerStateType.Idle, IdleState},
                { PlayerStateType.Move, MoveState},
                { PlayerStateType.Attack, AttackState},
                { PlayerStateType.Roll, RollState},
                { PlayerStateType.Hit, HitState},
                { PlayerStateType.Stunned, StunnedState},
                { PlayerStateType.Jump, JumpState},
                { PlayerStateType.Fall, FallState},
                { PlayerStateType.Death, DeathState},
                { PlayerStateType.Aim, AimState},
                { PlayerStateType.Equip, EquipState},
                { PlayerStateType.Unequip, UnequipState},
                { PlayerStateType.Crouch, CrouchState},
                { PlayerStateType.UI, UIState},
                { PlayerStateType.BowCharge, BowChargeState}
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
        PaletteSystem.instance.HandlePaletteLogic(this);

        // --- LOGIQUE D'OUVERTURE ---
        if (Input.MenuPressed || Input.InventoryPressed)
        {
            UIPanelType newType = Input.MenuPressed ? UIPanelType.PauseMenu : UIPanelType.Inventory;

            // SI on est déjà en dialogue, on sauvegarde cet état
            if (StateMachine.CurrentState == UIState && RequestedPanelType == UIPanelType.Dialogue)
            {
                _previousPanelType = UIPanelType.Dialogue;
            }

            RequestedPanelType = newType;
            StateMachine.ChangeState(PlayerStateType.UI);
            Input.UseMenuInput();
            Input.UseInventoryInput();
        }

        // --- LOGIQUE DE FERMETURE ---
        else if (StateMachine.CurrentState == UIState)
        {
            if (Input.CloseMenuPressed || Input.CloseInventoryPressed)
            {
                // Si on avait un dialogue en cours avant la pause
                if (_previousPanelType == UIPanelType.Dialogue)
                {
                    RequestedPanelType = UIPanelType.Dialogue;
                    _previousPanelType = null;

                    // On force le rafraîchissement de l'UIState sans repasser par Idle
                    UIState.Enter();
                }
                else if (RequestedPanelType != UIPanelType.Dialogue)
                {
                    StateMachine.ChangeState(PlayerStateType.Idle);
                }

                Input.UseCloseInventoryInput();
                Input.UseCloseMenuInput();
            }
        }
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
    public void AE_EquipWeapon() // AE pour Animation Event
    {
        // On délègue la logique à l'état actuel si c'est un état d'équipement
        if (StateMachine.CurrentState is EquipState equip)
        {            
            equip.HandleWeaponSwitch();
        }
    }

    public void AE_UnequipWeapon()
    {
        if (StateMachine.CurrentState is UnequipState unequip)
        {
            unequip.HandleWeaponRemoval();
        }
    }

    public void AE_OnAttackFinished()
    {
        if (StateMachine.CurrentState is AttackState attack)
        {
            attack.OnAnimationFinished();
        }
    }
    public void AE_OnRollEnd()
    {
        if (StateMachine.CurrentState is RollState rollState)
        {
            rollState.OnRollAnimationEnd();
        }
    }
    public void AE_OnHitAnimationEnd()
    {
        if (StateMachine.CurrentState is HitState hitState)
        {
            hitState.OnHitAnimationEnd();
        }
    }
    public void PrepareEquip(ItemData data)
    {
        // On cherche l'item correspondant dans la librairie
        PendingLibraryItem = equipmentLibrary.Get(data);

        if (PendingLibraryItem != null)
        {
            PendingWeaponItem = data;

            PendingWeaponType = data.handWeaponType;
            StateMachine.ChangeState(PlayerStateType.Equip);
        }
    }

    public float GetBaseWeaponDamage() => PendingWeaponItem.attackPoints;
}