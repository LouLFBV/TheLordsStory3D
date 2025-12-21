using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    public static PlayerStats instance;

    [Header("Others Elements References")]
    [SerializeField] private Animator animator;
    public ReputationData reputationData;
    private MoveBehaviour playerMovementScript;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    public float currentHealth;

    [SerializeField] private Image healthBarFill;

    [SerializeField] private float healthDecreaseRateForHungerAndThirst;

    [Header("Endurance")]

    [SerializeField] 
    private float enduranceRecoveryDelay = 2f;  // délai avant regen endurance
    private float enduranceRecoveryTimer = 0f;
    private bool canRecoverEndurance = true;
    private float maxEndurance = 100f;
    public float currentEndurance;
    public float coutDuSprint = 30f;

    [SerializeField] private Image enduranceBarFill;

    [SerializeField] private float enduranceDecreaseRateForHungerAndThirst;


    [Header("Armor")]
    public float currentArmourPointsPercant;
    public float currentArmourPointsContendant;
    public float currentArmourPointsTranchant;
    public float currentArmourPointsFire;
    public float currentArmourPointsIce;
    public float currentArmourPointsElectric;

    [SerializeField] private TextMeshProUGUI armorPercantText;
    [SerializeField] private TextMeshProUGUI armorContendantText;
    [SerializeField] private TextMeshProUGUI armorTranchantText;
    [SerializeField] private TextMeshProUGUI armorFireText;
    [SerializeField] private TextMeshProUGUI armorIceText;
    [SerializeField] private TextMeshProUGUI armorElectricText;


    [Header("Gold")]

    public int goldAmount;

    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip goldSound;


    [HideInInspector] public bool isDead = false;
    [HideInInspector] public EquipmentLibraryItem equipmentToEquip, equipmentToDesequip;
    [HideInInspector] public bool isEquiping = false;
    void Awake()
    {
        instance = this;
        playerMovementScript = GetComponent<MoveBehaviour>();
        currentHealth = maxHealth;
        currentEndurance = maxEndurance;
    }

    void Update()
    {
        if (currentEndurance <= 0)
        {
            canRecoverEndurance = false;
            enduranceRecoveryTimer += Time.deltaTime;

            // Après le délai, on peut régénérer 
            if (enduranceRecoveryTimer >= enduranceRecoveryDelay)
            {
                canRecoverEndurance = true;
                enduranceRecoveryTimer = 0f;
            }
        }
        else
        {
            // Si endurance > 0 on reset le timer et on peut toujours regen
            enduranceRecoveryTimer = 0f;
            canRecoverEndurance = true;
        }

        if (!playerMovementScript.isSprinting  && currentEndurance < maxEndurance && canRecoverEndurance)
        {
            UpdateEndurance(20f * Time.deltaTime); // regen endurance
        }
    }


    public void TakeDamage(float damage,DamageType damageType, bool overTime = false)
    {
        if (overTime)
        {
            currentHealth -= damage * Time.deltaTime;
        }
        else
        {
            switch(damageType)
            {
                case DamageType.Percant:
                    damage -= (currentArmourPointsPercant / 100);
                    break;
                case DamageType.Contendant:
                    damage -= (currentArmourPointsContendant / 100);
                    break;
                case DamageType.Tranchant:
                    damage -= (currentArmourPointsTranchant / 100);
                    break;
                case DamageType.Feu:
                    damage -= (currentArmourPointsFire / 100);
                    break;
                case DamageType.Glace:
                    damage -= (currentArmourPointsIce / 100);
                    break;
                case DamageType.Foudre:
                    damage -= (currentArmourPointsElectric / 100);
                    break;
            }
            currentHealth -= damage;
        }
        if (currentHealth <= 0 && !isDead)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthBar();
    }

    private void Die()
    {
        isDead = true;
        playerMovementScript.canMove = false;

        animator.SetTrigger("Die");
    }

    public void UpdateHealthBar()
    {
        healthBarFill.fillAmount = currentHealth / maxHealth;
    }


    public void UpdateEndurance(float amount)
    {
        currentEndurance = Mathf.Clamp(currentEndurance + amount, 0, maxEndurance);
        enduranceBarFill.fillAmount = currentEndurance / maxEndurance;
    }

    public void ConsumeItem(float health)
    {
        currentHealth = Mathf.Min(currentHealth + health, maxHealth);
        UpdateHealthBar();
    }

    public void AddGold(int amount)
    {
        goldAmount += amount;
        audioSource.PlayOneShot(goldSound);
        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        goldText.text = goldAmount.ToString() ;
    }

    public void UpddateArmorText()
    {
        armorPercantText.text = currentArmourPointsPercant.ToString() + "%";
        armorContendantText.text = currentArmourPointsContendant.ToString() + "%";
        armorTranchantText.text = currentArmourPointsTranchant.ToString() + "%";
        armorFireText.text = currentArmourPointsFire.ToString() + "%";
        armorIceText.text = currentArmourPointsIce.ToString() + "%";
        armorElectricText.text = currentArmourPointsElectric.ToString() + "%";
    }

    public void EquipAndActiveItem()
    {
        equipmentToEquip.itemPrefab.SetActive(true);
        equipmentToEquip = null;
    }
    public void DesequipAndDesactiveItem()
    {
        equipmentToDesequip.itemPrefab.SetActive(false);
        equipmentToDesequip = null;
    }

    public void ActiveIsEquiping()=> isEquiping = true;

    public void DesactiveIsEquiping() => isEquiping = false;
}
