using UnityEngine;

public class ArmorSystem : MonoBehaviour
{
    [Header("Physical Resistances")]
    public float armorTranchant = 10f;
    public float armorContendant = 5f;
    public float armorPercant = 0f; // Pas de résistance pour les dégâts perçants

    [Header("Elemental Resistances")]
    public float armorFeu = 0f;
    public float armorGlace = 0f;
    public float armorFoudre = 0f;

    public float CalculateReducedDamage(float rawDamage, DamageType type)
    {
        float reduction = 0;

        switch (type)
        {
            case DamageType.Tranchant: reduction = armorTranchant; break;
            case DamageType.Contendant: reduction = armorContendant; break;
            case DamageType.Feu: reduction = armorFeu; break;
            case DamageType.Percant: reduction = 0f; break; // Pas de réduction pour les dégâts perçants
            case DamageType.Classique: reduction = (armorTranchant + armorContendant) / 2f; break; // Moyenne des deux pour les dégâts classiques
            case DamageType.Glace: reduction = armorGlace; break;
            case DamageType.Foudre: reduction = armorFoudre; break;
        }

        // Exemple de calcul : Dégâts - Armure (avec un minimum de 1 dégât)
        return Mathf.Max(rawDamage - reduction, 1f);
    }
    public void UpdateArmor(DamageType type, float amount, bool isAdding)
    {
        float modifier = isAdding ? 1f : -1f;
        float value = amount * modifier;

        switch (type)
        {
            case DamageType.Tranchant: armorTranchant += value; break;
            case DamageType.Contendant: armorContendant += value; break;
            case DamageType.Percant: armorPercant += value; break;
            case DamageType.Feu: armorFeu += value; break;
            case DamageType.Glace: armorGlace += value; break;
            case DamageType.Foudre: armorFoudre += value; break;
        }
    }
}