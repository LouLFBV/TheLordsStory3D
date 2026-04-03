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

    [Header("Combat Settings")]
    [SerializeField] private List<EnemyWeaponSetup> weaponSetups;
    private Dictionary<AttackSO, EnemyWeaponSetup> _attackToWeaponMap;
    public Dictionary<AttackSO, WeaponDamageDetector> weaponDict;
    [SerializeField] private List<AttackSO> availableAttacks;
    public ItemData PendingWeaponItem { get; set; }

    [Header("Senses & AI")]
    public Transform target; // Le joueur
    public float AttackRadius = 2f;
    public NewEnemySO enemyData; 

    [Header("UI & Feedback")]
    [SerializeField] private GameObject lockOnIndicator;
    [SerializeField] private EnemyHealthUI healthUI;

    [Header("Systems (Shared with Player)")]
    public HealthSystem Health { get; private set; }
    public PoiseSystem Poise { get; private set; }
    public CombatSystem Combat { get; private set; }
    public DamageReceiver DmgReceiver { get; private set; }
    public AIManager AIManager { get; private set; }

    [Header ("Enemy States")]
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyFollowState FollowState { get; private set; }
    public EnemyOrbitState OrbitState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyHitState HitState { get; private set; }
    public EnemyStunnedState StunnedState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }

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
        AIManager = GetComponent<AIManager>();
        AIManager.Initialize(this); // On passe le controller à l'AI Manager pour qu'il puisse interagir avec les états et les systèmes de l'ennemi

        // 3. Création des instances d'états
        // On passera 'this' (le controller) à chaque état
        IdleState = new EnemyIdleState(this);
        FollowState = new EnemyFollowState(this);
        OrbitState = new EnemyOrbitState(this);
        AttackState = new EnemyAttackState(this);
        PatrolState = new EnemyPatrolState(this); // On le fera juste après
        HitState = new EnemyHitState(this);
        StunnedState = new EnemyStunnedState(this);
        DeathState = new EnemyDeathState(this);

        // 4. Setup de la State Machine
        var states = new Dictionary<EnemyStateType, EnemyState>
        {
            { EnemyStateType.Idle, IdleState },
            { EnemyStateType.Follow, FollowState },
            { EnemyStateType.Orbit, OrbitState },
            { EnemyStateType.Attack, AttackState },
            { EnemyStateType.Patrol, PatrolState },
            { EnemyStateType.Hit, HitState },
            { EnemyStateType.Stunned, StunnedState },
            { EnemyStateType.Death, DeathState }
        };

        StateMachine = new EnemyStateMachine(states);

        _attackToWeaponMap = new Dictionary<AttackSO, EnemyWeaponSetup>();
        foreach (var setup in weaponSetups)
        {
            foreach (var attack in setup.usableAttacks)
            {
                if (!_attackToWeaponMap.ContainsKey(attack))
                    _attackToWeaponMap.Add(attack, setup);
            }
        }
    }

    private void Start()
    {
        // 1. Recherche du joueur
        if (target == null && PlayerController.Instance != null)
            target = PlayerController.Instance.transform;

        if (enemyData == null)
            enemyData = AIManager.GetData();

        // 3. Initialisation de l'UI et du Lock
        if (healthUI != null) healthUI.Initialize(Health);
        SetLockOnIndicator(false);

        // 4. Lancement de l'IA
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
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        Vector3 dirToPlayer = (target.position - transform.position).normalized;

        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        // LOGIQUE DE TRANSITION :

        // 1. Si je suis en Idle ou Patrol et que je vois le joueur -> Poursuite
        if (distance <= enemyData.visionRange && angleToPlayer < enemyData.visionAngle / 2f)
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
    public AttackSO GetBestAttack()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        List<AttackSO> possibleAttacks = new List<AttackSO>();

        foreach (var attack in availableAttacks)
        {
            // 1. Check de la distance
            bool isInRange = distance >= attack.minDistance && distance <= attack.maxDistance;

            // 2. Check du cooldown
            bool isOffCooldown = Time.time >= attack.lastUsedTime + attack.attackCooldown;

            if (isInRange && isOffCooldown)
            {
                possibleAttacks.Add(attack);
            }
        }

        if (possibleAttacks.Count > 0)
        {
            // On prend une attaque au hasard parmi celles qui sont valides
            AttackSO selected = possibleAttacks[Random.Range(0, possibleAttacks.Count)];
            selected.lastUsedTime = Time.time; // On lance le cooldown
            return selected;
        }

        return null; // Aucune attaque n'est prête ou à portée
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

    public void EquipWeapon(AttackSO weapon)
    {
        if (weaponDict.TryGetValue(weapon, out var detector))
        {
            Combat.UpdateWeaponDetector(detector);
        }
    }
    public void PrepareAttack(AttackSO attack)
    {
        if (_attackToWeaponMap.TryGetValue(attack, out var setup))
        {
            // On met à jour l'arme actuelle (pour les dégâts de base)
            PendingWeaponItem = setup.weaponData;

            // On met à jour le détecteur physique (la hitbox)
            Combat.UpdateWeaponDetector(setup.detector);
        }
    }

    private void OnEnable()
    {
        if (Health != null)
            Health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (Health != null)
            Health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        // On force le passage à l'état de mort, peu importe l'état actuel

        NewQuestManager.instance.UpdateQuestProgress(AIManager.GetData().enemyType.ToString(), 1);
        StateMachine.ChangeState(EnemyStateType.Death);
    }

    public void SetLockOnIndicator(bool isLocked)
    {
        if (lockOnIndicator != null)
            lockOnIndicator.SetActive(isLocked);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.visionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);

        // Cône de vision
        Gizmos.color = Color.yellow;

        Vector3 leftRay = Quaternion.Euler(0, -enemyData.visionAngle / 2f, 0) * transform.forward;
        Vector3 rightRay = Quaternion.Euler(0, enemyData.visionAngle / 2f, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftRay * enemyData.visionRange);
        Gizmos.DrawRay(transform.position, rightRay * enemyData.visionRange);
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

[System.Serializable]
public class EnemyWeaponSetup
{
    public ItemData weaponData;          // Contient les points d'attaque (ex: 15)
    public WeaponDamageDetector detector; // Le script sur l'objet physique
    public List<AttackSO> usableAttacks;  // Les attaques que CETTE arme peut faire
}