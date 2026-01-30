using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Interactable Icon Database")]
public class InteractableIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public InteractableObjectType type;
        public Sprite sprite;
    }

    public List<Entry> entries;

    public Sprite Get(InteractableObjectType type)
    {
        foreach (var e in entries)
            if (e.type == type)
                return e.sprite;

        return null;
    }
}

public enum InteractableObjectType
{
    None,
    Chest,
    Locked,
    Key,
    Axe,
    Door,
    PNJ,
    Gold,
    Pickaxe,
    Hand
}
