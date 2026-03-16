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

    [Header("Combat Settings")]
    [SerializeField] private List<AttackSO> availableAttacks;
    public ItemData PendingWeaponItem { get; set; }

    [Header("Senses & AI")]
    public Transform Target; // Le joueur
    public float DetectionRadius = 10f;
    public float AttackRadius = 2f;

    [Header("UI & Feedback")]
    [SerializeField] private GameObject lockOnIndicator;
    [SerializeField] private EnemyHealthUI healthUI;

    [Header("Systems (Shared with Player)")]
    public HealthSystem Health { get; private set; }
    public PoiseSystem Poise { get; private set; }
    public CombatSystem Combat { get; private set; }
    public DamageReceiver DmgReceiver { get; private set; }

    [Header ("Enemy States")]
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
        // Initialisation de l'UI de vie
        if (healthUI != null) healthUI.Initialize(Health);

        // On cache l'indicateur de lock au début
        SetLockOnIndicator(false);
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

        // LOGIQUE DE TRANSITION :

        // 1. Si je suis en Idle ou Patrol et que je vois le joueur -> Poursuite
        if (distance <= DetectionRadius)
        {
            if (StateMachine.CurrentState == IdleState || StateMachine.CurrentState == PatrolState)
            {
                StateMachine.ChangeState(EnemyStateType.Follow);
            }
        }

        // 2. Si je suis en Idle trop longtemps -> Patrouille (Optionnel)
        if (StateMachine.CurrentState == IdleState)
        {
            // Tu peux ajouter un petit timer ici pour passer en Patrol automatiquement
            StateMachine.ChangeState(EnemyStateType.Patrol);
        }
    }

    // Méthode pour choisir une attaque
    public AttackSO GetRandomAttack()
    {
        if (availableAttacks == null || availableAttacks.Count == 0) return null;
        return availableAttacks[Random.Range(0, availableAttacks.Count)];
    }

    // Implémentation de ICombatant
    public float GetBaseWeaponDamage()
    {
        // On utilise les dégâts de ton ItemData (arme) ou de ton EnemySO
        return PendingWeaponItem != null ? PendingWeaponItem.attackPoints : 10f;
    }

    // On fait le pont entre les events de l'Animator et le CombatSystem
    public void AE_HitboxOpen() => Combat.AE_HitboxOpen();
    public void AE_HitboxClose() => Combat.AE_HitboxClose();
    // --- Animation Events (Même logique que le joueur) ---
    public void AE_OnAttackFinished() => (StateMachine.CurrentState as EnemyAttackState)?.OnAnimationFinished();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
    }

    public void SetLockOnIndicator(bool isLocked)
    {
        if (lockOnIndicator != null)
            lockOnIndicator.SetActive(isLocked);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (StateMachine != null && StateMachine.CurrentState != null)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f,
                $"State: {StateMachine.CurrentState.GetType().Name}");
        }
    }
#endif
}