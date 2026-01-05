using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class EnemyParent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected EnemySO enemyData;
    
    [HideInInspector]public NavMeshAgent agent;
    protected Animator animator;


    [SerializeField] protected Image healthBar;
    [SerializeField] protected GameObject vie;

    protected float currentHealth;

    protected Transform player;
    protected PlayerStats playerStats;

    [SerializeField] protected GameObject itemToDrop;

    protected bool playerInSight = false;

    [Header("Type Defense and Attack")]
    [SerializeField] protected DamageType[] defensePointFortType;
    [SerializeField] protected DamageType[] defensePointFaibleType;
    [SerializeField] protected float pourcentageOfResistance = 0.2f; // 20% de résistance par rapport au type de défense

    public bool isAttacking = false;

    private bool isDead; 
    public bool IsDead
    {
        get => isDead;  
        protected set     
        {
            isDead = value;                        
            animator.SetBool("IsDead", isDead);    
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentHealth = enemyData.pvMax;

        isDead = animator.GetBool("IsDead");
    }

    void Start()
    {
        playerStats = PlayerStats.instance;
        player = playerStats.transform;
    }

    public virtual void TakeDamage(float damage, DamageType damageType)
    {
        if (IsDead) return;

        foreach (DamageType type in defensePointFortType)
        {
            if (type == damageType)
            {
                damage *= (1f - pourcentageOfResistance);
                break;
            }
        }
        foreach (DamageType type in defensePointFaibleType)
        {
            if (type == damageType)
            {
                damage *= (1f + pourcentageOfResistance);
                break;
            }
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (currentHealth < 0.001f)
            currentHealth = 0f;
        UpdateLife();

        playerInSight = true;

        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
           animator.SetTrigger("GetHit");
        }
    }


    protected virtual void UpdateLife()
    {
        healthBar.fillAmount = currentHealth / enemyData.pvMax;
    }

    protected abstract void Die();

    public abstract void UpdateSpeedWitchCoefficient(float speedCoefficient);
}



[System.Serializable]
public class BossAttack
{
    public GameObject gameObjectAttack;
    public HitBoxAttack hitBoxAttack;
    public int damage;
    public int boostDamage;
    public string animTriggerName;
    public float distanceMin, distanceMax;
    public float timeBetweenAttacks = 3f;
    [HideInInspector] public float nextAttackTime;
    public DamageType damageType;
    public bool isFireBreath = false;

    [Header("Fire Ball")]
    public GameObject fireBallPrefab;
    public Transform firePoint;
    public float fireBallSpeed = 10f;
}