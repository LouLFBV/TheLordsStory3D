using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponCombatData", menuName = "Combat/Weapon Data")]
public class WeaponCombatData : ScriptableObject
{
    [Header("Combo Configuration")]
    public AttackSO startingAttack; // La première attaque (ex: Light_1)

    // Tu peux ajouter ici des sons d'impact spécifiques à l'arme
}