using UnityEngine;

public class CanHarvest : MonoBehaviour
{
    [SerializeField] private InteractSystem interactBehaviour;

    [SerializeField] private bool isAxe = false;

    [SerializeField] private bool isPickaxe = false;

    private void OnEnable()
    {
        Debug.Log("Enabling CanHarvest: " + gameObject.name);
        if (isAxe)
        {
            interactBehaviour.canAxe = true;
        }
        else if (isPickaxe)
        {
            interactBehaviour.canPickaxe = true;
        }
    }
    private void OnDisable()
    {
        if (isAxe)
        {
            interactBehaviour.canAxe = false;
        }
        else if (isPickaxe)
        {
            interactBehaviour.canPickaxe = false;
        }
    }
}

