using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private ICombatant _owner; // Peut être le joueur ou un ennemi !
    
    private Animator _animator;
    private WeaponDamageDetector _weaponDetector;
    private AttackSO _currentAttackData;
    public int attackLayer = 9;

    private bool canCombo;

    private void Awake()
    {
        _owner = GetComponent<ICombatant>();
        _animator = GetComponent<Animator>();
    }
    public void ExecuteAttack(AttackSO attack)
    {
        _currentAttackData = attack;
        canCombo = false;

        _animator.applyRootMotion = true;
        _animator.Play(attack.AnimationHash, attackLayer, 0f);
    }

    // --- Méthodes appelées par Animation Events ---

    public void AE_EnableCombo()
    {
        canCombo = true;
        Debug.Log("Fenêtre de combo ouverte !");
    }

    public void AE_HitboxOpen()
    {
        if (_weaponDetector != null && _currentAttackData != null && _owner != null)
        {
            // On demande les dégâts de base à l'interface, peu importe qui c'est
            float weaponDamage = _owner.GetBaseWeaponDamage();
            float finalDamage = weaponDamage * _currentAttackData.damageMultiplier;

            _weaponDetector.SetDamageFrame(finalDamage);
            _weaponDetector.ToggleCollider(true);
        }
    }

    public void AE_HitboxClose()
    {
        _weaponDetector?.DisableDamage();
        _weaponDetector?.ToggleCollider(false);
    }

    public bool CanComboNext() => canCombo;

    public void UpdateWeaponDetector(WeaponDamageDetector newDetector)
    {
        _weaponDetector = newDetector;
    }
}