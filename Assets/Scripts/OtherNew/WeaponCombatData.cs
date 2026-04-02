using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponCombatData", menuName = "Combat/Weapon Data")]
public class WeaponCombatData : ScriptableObject
{
    [Header("Combo Configuration")]
    public AttackSO startingAttack;
    public AttackSO specialAttack;

    // Tu peux ajouter ici des sons d'impact spécifiques à l'arme
}