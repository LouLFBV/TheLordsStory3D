using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using System;

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

    public bool isAttacking = false;
    public bool canAttack;
    private ItemData weaponActive;

    #region PlayerInput
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    private bool attackInput = false;
    private bool attackSpecialInput = false;
    #endregion

    void Update()
    {
        if (CanAttack() && !bowBehaviour.chargeBow)
        {
            weaponActive = palette.isEquippedWeapon1 ? palette.equipmentWeapon1Item : palette.equipmentWeapon2Item;
            if (attackInput)
            {
                if (weaponActive.handWeaponType == HandWeapon.Bow && bowBehaviour.VerifIfCanShoot())
                {
                    bowBehaviour.PrepareArrow();
                }
                else if (weaponActive.handWeaponType != HandWeapon.Bow)
                {
                    isAttacking = true;
                    animator.SetTrigger("Attack");
                }
            }
            else if (attackSpecialInput)
            {
                if (weaponActive.handWeaponType != HandWeapon.Bow)
                {
                    isAttacking = true;
                    animator.SetTrigger("AttackSpecial");
                }
            }
        }
        else if (!attackInput && bowBehaviour.chargeBow && CanAttack() && bowBehaviour.canShoot)
        {
            bowBehaviour.ShootArrow();
        }
    }
    private void OnEnable()
    {
        playerInput.actions["Attack"].performed += OnAttackPerformed;
        playerInput.actions["Attack"].canceled += OnAttackCanceled; 
        playerInput.actions["AttackSpecial"].performed += OnAttackSpecialPerformed;
        playerInput.actions["AttackSpecial"].canceled += OnAttackSpecialCanceled;
    }

   

    private void OnDisable()
    {
        playerInput.actions["Attack"].performed -= OnAttackSpecialPerformed;
        playerInput.actions["Attack"].canceled -= OnAttackCanceled;
        playerInput.actions["AttackSpecial"].performed -= OnAttackSpecialPerformed;
        playerInput.actions["AttackSpecial"].canceled -= OnAttackSpecialCanceled;
    }


    private void OnAttackSpecialPerformed(InputAction.CallbackContext ctx)
    {
        attackSpecialInput = true;
    }
    private void OnAttackSpecialCanceled(InputAction.CallbackContext context)
    {
        attackSpecialInput = false;
    }
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        attackInput = true;
    }
    private void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
        attackInput = false;
    }
    public bool CanAttack()
    {
        return (palette.isEquippedWeapon1 || palette.isEquippedWeapon2) && !isAttacking && !uiManager.atLeashOnePanelOpened && !interactBehaviour.isBusy && !playerStats.isDead && canAttack;
    }

    public void EnableRootMotion() => animator.applyRootMotion = true;
    
    public void AttackFinished()
    {
        Debug.Log("Attack Finished");
        isAttacking = false;
       //animator.applyRootMotion = false;
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
