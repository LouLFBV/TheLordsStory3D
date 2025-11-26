using UnityEngine;
using System.Collections;

public class InteractBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CraftingTableParent craftingTableParent;
    [SerializeField] private MoveBehaviour player;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Equipment equipmentSystem;
    [SerializeField] private EquipmentLibrary equipmentLibrary;
    [SerializeField] private AudioSource audioSource;
    [HideInInspector] public bool isBusy = false;

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

        playerAnimator.SetTrigger("PickUp");
        player.canMove = false;
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
    IEnumerator BreakHarvestable()
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
        Destroy(currentlyHarveting.gameObject);
        RespawnObject(currentlyHarveting.transform);
    }
    public void ReEnablePlayerMouvement()
    {
        EnableToolGameObjectFromTool(currentTool);
        player.canMove = true;
        isBusy = false;
    }
    
    public void EnableTwoHand()
    {
        if (equipmentToDesactiveAndActive != null && equipmentToDesactiveAndActive.itemData.handWeaponType == HandWeapon.TwoHanded)
        {
            playerAnimator.SetBool("IsTwoHandedWeapon", true);
        }
    }
    public void DiseableTwoHand()
    {
        if(equipmentToDesactiveAndActive == null)
            return;
        if (equipmentToDesactiveAndActive != null && equipmentToDesactiveAndActive.itemData.handWeaponType == HandWeapon.TwoHanded)
        {
            playerAnimator.SetBool("IsTwoHandedWeapon", false);
        }
    }
    public void AddItemToInventory()
    {
        if (currentItem.itemData.itemType == ItemType.Recipe)
        {
            currentItem.GetComponent<BookRecipe>().OpenCanvasRecipeBook();
            if (currentItem.itemData.recipe.craftableItem.itemType == ItemType.Consumable)
                craftingTableParent.recetteDeLObjectCooking.Add(currentItem.itemData.recipe);
            else
                craftingTableParent.recetteDeLObjectCrafting.Add(currentItem.itemData.recipe);
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
