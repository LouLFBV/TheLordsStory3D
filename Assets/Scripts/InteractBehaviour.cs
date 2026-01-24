using UnityEngine;
using System.Collections;

public class InteractBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AllRecipeData allRecipeData;
    [SerializeField] private MoveBehaviour player;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Equipment equipmentSystem;
    [SerializeField] private EquipmentLibrary equipmentLibrary;
    [SerializeField] private AudioSource audioSource;
    [HideInInspector] public bool isBusy = false;
    [SerializeField] private PlayerInteractor playerInteractor;

    [Header("Tools Configuration")]
    [SerializeField] private GameObject pickaxeVisual;
    [SerializeField] private AudioClip pickaxeSound;
    [SerializeField] private GameObject axeVisual;
    public bool canAxe = false;
    [SerializeField] private AudioClip axeSound;
    public bool canPickaxe = false;


    [Header("Other")]
    [SerializeField] private AudioClip pickUpSound;

    private Item currentItem;
    private Harvestable currentHarvestable;
    private Tool currentTool;

    private EquipmentLibraryItem equipmentToDesactiveAndActive;

    private Vector3 spawnItemOffset = new Vector3(0, 0.5f, 0);
    public void DoPickUp(Item item)
    {
        if (isBusy)return;

        isBusy = true;
        if (inventory.IsFull())
        {
            Debug.Log("Inventory is full, can't pick up : " +item.name);
            return;
        }
        currentItem = item;


        foreach (EquipmentLibraryItem itemLibrary in equipmentLibrary.content)
        {
            if (itemLibrary.itemPrefab.activeSelf)
            {
                equipmentToDesactiveAndActive = itemLibrary;
            }
        }

        Debug.Log("Picking up item: " + item.name);
        player.StopPlayer();
        playerAnimator.SetTrigger("PickUp");
    }

    public void DoHarvest(Harvestable harvestable)
    {
        if (isBusy)return;
        
        isBusy = true;
        currentTool = harvestable.tool;
        EnableToolGameObjectFromTool(currentTool);  
        currentHarvestable = harvestable;
        playerAnimator.SetTrigger("Harvest");
        player.canMove = false;
    }

    //Coroutine appelé depuis l'animation "Haversitng"
    public IEnumerator BreakHarvestable()
    {
        Harvestable currentlyHarveting = currentHarvestable;
        currentlyHarveting.gameObject.layer = LayerMask.NameToLayer("Default");
        if (currentlyHarveting.disableKinematicOnHarvest)
        {
            Rigidbody rigidbody = currentlyHarveting.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.AddForce(transform.forward * 800,ForceMode.Impulse);
        }

        yield return new WaitForSeconds(currentlyHarveting.destroyDelay);
        for (int i = 0; i < currentlyHarveting.harvestableItems.Length; i++)
        {
            Ressource ressource = currentlyHarveting.harvestableItems[i];
            if (Random.Range(0, 101) <= ressource.dropChance)
            {
                GameObject instantiatedRessource = Instantiate(ressource.itemData.prefab);

                // Rayon max du décalage (à ajuster)
                float spawnRadius = 0.2f;

                // Génère un décalage aléatoire dans un petit rayon (sur le sol uniquement, Y = 0)
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0f,
                    Random.Range(-spawnRadius, spawnRadius)
                );

                // Position finale = position du harvesting + offset initial + petit décalage aléatoire
                instantiatedRessource.transform.position = currentlyHarveting.transform.position + spawnItemOffset + randomOffset;
            }
        }
        Debug.Log("Harvested: " + currentlyHarveting.name);
        if (currentlyHarveting.TryGetComponent<WorldObjectID>(out var worldID))
        {
            Debug.Log("Registering collected object with ID: " + worldID.uniqueID);
            WorldStateManager.Instance.RegisterCollectedObject(worldID.uniqueID);
        }
        else
        {
            Debug.Log("No WorldObjectID found on harvested object: " + currentlyHarveting.name);
        }

        Destroy(currentlyHarveting.gameObject);
        RespawnObject(currentlyHarveting.transform);
    }
    public void ReEnablePlayerMouvement()
    {
        EnableToolGameObjectFromTool(currentTool);
        player.StartPlayer();
        isBusy = false;
    }

    public void SetCurrentEquippedItem(EquipmentLibraryItem equippedItem)
    {
        equipmentToDesactiveAndActive = equippedItem;
    }

    public void EnableTwoHand()
    {
        if (equipmentToDesactiveAndActive == null) return;
        if (playerAnimator == null) return; // <- protection supplémentaire
        if (equipmentToDesactiveAndActive.itemData == null)return;
        
        if (equipmentToDesactiveAndActive.itemData.handWeaponType == HandWeapon.OneHanded) return;

        if (equipmentToDesactiveAndActive.itemData.handWeaponType != HandWeapon.TwoHanded) return;

        playerAnimator.SetBool("IsTwoHandedWeapon", true);
        
    }
    public void DisableTwoHand()
    {
        if (equipmentToDesactiveAndActive == null) return;
        if (playerAnimator == null) return; // <- protection supplémentaire
        if (equipmentToDesactiveAndActive.itemData == null) return;
        if (equipmentToDesactiveAndActive.itemData.handWeaponType == HandWeapon.OneHanded) return;
        if (equipmentToDesactiveAndActive.itemData.handWeaponType != HandWeapon.TwoHanded) return;

        playerAnimator.SetBool("IsTwoHandedWeapon", false);
        
    }
    public void AddItemToInventory()
    {
        if (currentItem.itemData.itemType == ItemType.Recipe)
        {
            currentItem.GetComponent<BookRecipe>().OpenCanvasRecipeBook();
            if (currentItem.itemData.recipe.craftableItem.itemType == ItemType.Consumable)
                allRecipeData.recetteDeLObjectCooking.Add(currentItem.itemData.recipe);
            else
                allRecipeData.recetteDeLObjectCrafting.Add(currentItem.itemData.recipe);
        }
        else if (currentItem.itemData.itemType == ItemType.Map)
        {
            MapManager.instance.AddIconeMap(currentItem.itemData.iconeMap);
        }
        else
        {
            for (int i = 0; i < currentItem.amount; i++)
            {
                if (!inventory.IsFull())
                {
                    inventory.AddItem(currentItem.itemData);
                    QuestManager.instance.UpdateQuestProgress("", 1, currentItem.itemData);
                }
            }
        }

        audioSource.PlayOneShot(pickUpSound);

        var interact = currentItem.GetComponent<IInteractable>();
        interact?.SetTargeted(false, player.transform);
        if (currentItem.TryGetComponent<WorldObjectID>(out var id))
        {
            WorldStateManager.Instance.RegisterCollectedObject(id.uniqueID);
        }

        Destroy(currentItem.gameObject);

        RespawnObject(currentItem.transform);
    }

    private void EnableToolGameObjectFromTool(Tool toolType)
    {
        switch (toolType)
        {
            case Tool.Pickaxe:
                audioSource.clip = pickaxeSound;
                break;
            case Tool.Axe:
                audioSource.clip = axeSound;
                break;
        }
    }

    public void PlayHarvestingSoundEffect()
    {
        audioSource.Play();
    }

    private void RespawnObject(Transform objectToRespawn)
    {
        Transform parent = objectToRespawn.transform.parent;
        if (parent != null)
        {
            ObjectsSpawner objectsSpawner = parent.GetComponent<ObjectsSpawner>();
            if (objectsSpawner != null)
            {
                objectsSpawner.OnObjectCollected();
            }
        }
    }
}
