using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour, ICombatant
{
    [Header("Core Components")]
    public EnemyStateMachine StateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    [Header("Data & Stats")]
    [SerializeField] private EnemySO enemyData;
    // On garde ton SO pour les PV max et le type !

    [Header("Senses & AI")]
    public Transform Target; // Le joueur
    public float DetectionRadius = 10f;
    public float AttackRadius = 2f;

    [Header("Systems (Shared with Player)")]
    public HealthSystem Health { get; private set; }
    public PoiseSystem Poise { get; private set; }
    public CombatSystem Combat { get; private set; }
    public DamageReceiver DmgReceiver { get; private set; }

    // On prépare les slots pour les états
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyFollowState FollowState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }

    private void Awake()
    {
        // 1. Initialisation des composants physiques
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();

        // 2. Initialisation des systèmes de combat (les mêmes que le joueur !)
        Health = GetComponent<HealthSystem>();
        Poise = GetComponent<PoiseSystem>();
        Combat = GetComponent<CombatSystem>();
        DmgReceiver = GetComponent<DamageReceiver>();

        // 3. Création des instances d'états
        // On passera 'this' (le controller) à chaque état
        IdleState = new EnemyIdleState(this);
        FollowState = new EnemyFollowState(this);
        AttackState = new EnemyAttackState(this);
        PatrolState = new EnemyPatrolState(this); // On le fera juste après

        // 4. Setup de la State Machine
        var states = new Dictionary<EnemyStateType, EnemyState>
        {
            { EnemyStateType.Idle, IdleState },
            { EnemyStateType.Follow, FollowState },
            { EnemyStateType.Attack, AttackState },
            { EnemyStateType.Patrol, PatrolState }
        };

        StateMachine = new EnemyStateMachine(states);
    }

    private void Start()
    {
        // On cherche le joueur automatiquement au début
        if (Target == null && PlayerController.Instance != null)
            Target = PlayerController.Instance.transform;

        StateMachine.Initialize(EnemyStateType.Idle);
    }

    private void Update()
    {
        StateMachine.Update();

        // C'est ici que l'AI Manager interviendrait, 
        // mais pour l'instant on peut mettre une logique simple :
        CheckForPlayer();
    }

    private void FixedUpdate() => StateMachine.FixedUpdate();

    private void CheckForPlayer()
    {
        if (Target == null) return;

        float distance = Vector3.Distance(transform.position, Target.position);

        // Logique de transition ultra basique pour tester :
        if (distance <= DetectionRadius && StateMachine.CurrentState == IdleState)
        {
            StateMachine.ChangeState(EnemyStateType.Follow);
        }
    }

    // --- Animation Events (Même logique que le joueur) ---
   public void AE_OnAttackFinished() => (StateMachine.CurrentState as EnemyAttackState)?.OnAnimationFinished();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
    }

    public float GetBaseWeaponDamage()
    {
        throw new System.NotImplementedException();
    }
}