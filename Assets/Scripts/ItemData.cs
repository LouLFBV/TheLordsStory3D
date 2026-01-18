using UnityEngine;

[CreateAssetMenu(fileName = "Item",menuName = "Items/New Item")]
public class ItemData : ScriptableObject
{
    [Header("Data")]
    public string itemID;
    public string itemName;
    [TextArea]
    public string description;
    public Sprite visual;
    public GameObject prefab;
    public bool stackable;
    public int maxStack;
    public int prix = 10;
    public int levelAmelioration = 0;
    public RecipeData recipe;
    public GameObject iconeMap;

    [Header("Effects")]
    public float healthEffect;

    [Header("Armor Stats")]
    public float armorPoints;
    public DamageType armorType;

    [Header("Attack Stats")]
    public float attackPoints;
    public HandWeapon handWeaponType;
    public DamageType damageType;

    [Header("Bow Stats")]
    public float rangeMin;
    public float damageMin;
    public float rangeMax;
    public float damageMax;
    [HideInInspector] public float range;
    [HideInInspector] public float damage;

    [Header("Types")]
    public ItemType itemType;
    public EquipmentType equipmentType;

    [Header("Camera Shake")]
    public float cameraShakeIntensity = 0.15f;
    public float cameraShakeDuration = 0.2f;
}

public enum ItemType
{
    Consumable,
    Equipment,
    QuestItem,
    Ressource,
    Key,
    Constructible,
    Destructible,
    Recipe,
    Map,
    None
}

public enum EquipmentType
{
    Head,
    Chest,
    Legs,
    Feet,
    Hands,
    Weapon,
    Shield,
    None,
    Arrow
}

public enum HandWeapon
{
    OneHanded,
    TwoHanded,
    Bow
}


public enum DamageType
{
    Classique,
    Percant,
    Contendant,
    Tranchant,
    Feu,
    Glace,
    Foudre,
    Magie
}