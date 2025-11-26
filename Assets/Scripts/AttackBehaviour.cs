using UnityEngine;
using System.Linq;

public class AttackBehaviour : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InteractBehaviour interactBehaviour;
    [SerializeField] private Animator animator;
    [SerializeField] private Palette palette;
    [SerializeField] private EquipmentLibrary equipmentLibrary;
    [SerializeField] private BowBehaviour bowBehaviour;

    private bool isAttacking = false;
    public bool canAttack;
    private ItemData weaponActive;

    #region PlayerControls
    private PlayerControls controls;
    private bool attackInput;
    #endregion

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Attack.performed += ctx => attackInput = true;
        controls.Player.Attack.canceled += ctx => attackInput = false;
    }

    void Update()
    {
        if (attackInput && CanAttack() && !bowBehaviour.chargeBow)
        {
            weaponActive = palette.isEquippedWeapon1 ? palette.equipmentWeapon1Item : palette.equipmentWeapon2Item;

            if (weaponActive.handWeaponType == HandWeapon.Bow && bowBehaviour.VerifIfCanShoot())
            {
                bowBehaviour.PrepareArrow();
            }
            else if(weaponActive.handWeaponType != HandWeapon.Bow)
            {
                isAttacking = true;
                animator.SetTrigger("Attack");
            }
        }
        else if (!attackInput && bowBehaviour.chargeBow && CanAttack() && bowBehaviour.canShoot)
        {
            bowBehaviour.ShootArrow();
        }
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();
    public bool CanAttack()
    {
        return (palette.isEquippedWeapon1 || palette.isEquippedWeapon2) && !isAttacking && !uiManager.atLeashOnePanelOpened && !interactBehaviour.isBusy && !playerStats.isDead && canAttack;
    }
    public void AttackFinished()
    {
        isAttacking = false;
    }

    public void AttackEnable()
    {
        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == weaponActive).First();
        equipmentLibraryItem.itemPrefab.GetComponent<BoxCollider>().enabled = true;
        equipmentLibraryItem.itemPrefab.GetComponent<HitBoxWeapon>().enabled = true;

    }
    public void AttackDisable()
    {
        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(x => x.itemData == weaponActive).First();
        equipmentLibraryItem.itemPrefab.GetComponent<BoxCollider>().enabled = false;
        equipmentLibraryItem.itemPrefab.GetComponent<HitBoxWeapon>().enabled = false;
    }
}
