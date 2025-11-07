using TMPro;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private Palette palette;

    [SerializeField]
    private float interactRange = 2.6f;

    [SerializeField]
    private GameObject pointDeCollecte;

    [SerializeField]
    private TextMeshProUGUI pointDeCollecteText;

    public InteractBehaviour playerInteractBehaviour;

    [SerializeField]
    private LayerMask layerMask;
    void Update()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red);

        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange, layerMask))
        {
            pointDeCollecte.SetActive(true);
            ChangeText(hit);

            InteractionWithE(hit);
        }
        else
        {
            pointDeCollecte.SetActive(false);
        }   
    }

   private void ChangeText(RaycastHit hit)
    {
        GameObject hitGameObject = hit.transform.gameObject;
        switch (hit.transform.tag)
        {
            case "Item":
                pointDeCollecteText.text = "Appuyez sur E pour rammasser ";
                break;
            case "Harvestable":
                if (hitGameObject.GetComponent<Harvestable>().tool == Tool.Axe && !playerInteractBehaviour.canAxe)
                {
                    pointDeCollecteText.text = "(Hache requise)";
                }
                else if (hitGameObject.GetComponent<Harvestable>().tool == Tool.Pickaxe && !playerInteractBehaviour.canPickaxe)
                {
                    pointDeCollecteText.text = "(Pioche requise)";
                }
                else
                {
                    pointDeCollecteText.text = "Appuyez sur E pour détruire";
                }
                break;
            case "PoigneeDePorte":
                if (hitGameObject.GetComponent<Door>().isAnimating)
                    pointDeCollecte.SetActive(false);
                else if (hitGameObject.GetComponent<Door>().isOpen)
                    pointDeCollecteText.text = "Appuyez sur E pour fermer la porte";
                else if (hitGameObject.GetComponent<Door>().isLocked)
                {
                    if (palette.equipmentObject1Item == null)
                    {
                        if (palette.equipmentObject2Item != null)
                        {
                            if (palette.equipmentObject2Item.itemType == ItemType.Key)
                            {
                                pointDeCollecteText.text = "Appuyez sur E pour essayer d'ouvrir la porte (clé requise)";
                            }
                        }
                        else
                        {
                            pointDeCollecteText.text = "Porte verrouillée";
                        }
                    }
                    else if (palette.equipmentObject2Item == null)
                    {
                        if (palette.equipmentObject1Item != null)
                        {
                            if (palette.equipmentObject1Item.itemType == ItemType.Key)
                            {
                                pointDeCollecteText.text = "Appuyez sur E pour essayer d'ouvrir la porte (clé requise)";
                            }
                        }
                        else
                        {
                            pointDeCollecteText.text = "Porte verrouillée";
                        }
                    }
                }
                else
                    pointDeCollecteText.text = "Appuyez sur E pour ouvrir la porte";
                break;
            case "Chest":
                if (!hitGameObject.GetComponent<Chest>().isOpen)
                {
                    if (!hitGameObject.GetComponent<Chest>().isLocked)
                        pointDeCollecteText.text = "Appuyez sur E pour ouvrir le coffre";
                    else 
                    {
                        if (palette.equipmentObject1Item == null)
                        {
                            Debug.Log("EquipmentObject1Item is null");
                            if (palette.equipmentObject2Item != null)
                            {
                                if (palette.equipmentObject2Item.itemType == ItemType.Key)
                                {
                                    pointDeCollecteText.text = "Appuyez sur E pour essayer d'ouvrir le coffre (clé requise)";
                                }
                            }
                            else
                            {
                                pointDeCollecteText.text = "Coffre verrouillé";
                            }
                        }
                        else if (palette.equipmentObject2Item == null)
                        {
                            Debug.Log("EquipmentObject2Item is null");
                            if (palette.equipmentObject1Item != null)
                            {
                                if (palette.equipmentObject1Item.itemType == ItemType.Key)
                                {
                                    pointDeCollecteText.text = "Appuyez sur E pour essayer d'ouvrir le coffre (clé requise)";
                                }
                            }
                            else
                            {
                                pointDeCollecteText.text = "Coffre verrouillé";
                            }
                        }
                    }
                }
                else
                {
                    pointDeCollecte.SetActive(false);
                }
                break;
            case "PNJ":
                if (hitGameObject.GetComponent<PNJ>())
                {
                    if (!hitGameObject.GetComponent<PNJ>().isOnDial && Time.time - hitGameObject.GetComponent<PNJ>().dialogueEndTime > hitGameObject.GetComponent<PNJ>().inputCooldown)
                        pointDeCollecteText.text = "Appuyez sur E pour parler";
                    else
                        pointDeCollecte.SetActive(false);
                }
                break;
            case "Marchand":
                if (hitGameObject.GetComponent<Marchand>())
                {
                    if (!hitGameObject.GetComponent<Marchand>().isOnDial)
                        pointDeCollecteText.text = "Appuyez sur E pour parler";
                    else
                        pointDeCollecte.SetActive(false);
                }
                break;
            case "PNJAcheteur":
                if (hitGameObject.GetComponent<PNJAcheteur>())
                {
                    if (!hitGameObject.GetComponent<PNJAcheteur>().isOnDial && Time.time - hitGameObject.GetComponent<PNJAcheteur>().dialogueEndTime > hitGameObject.GetComponent<PNJAcheteur>().inputCooldown)
                        pointDeCollecteText.text = "Appuyez sur E pour parler";
                    else
                        pointDeCollecte.SetActive(false);
                }
                break;
            case "Forgeron":
                if (hitGameObject.GetComponent<Forgeron>())
                {
                    if (!hitGameObject.GetComponent<Forgeron>().isOnDial && Time.time - hitGameObject.GetComponent<Forgeron>().dialogueEndTime > hitGameObject.GetComponent<Forgeron>().inputCooldown)
                        pointDeCollecteText.text = "Appuyez sur E pour parler";
                    else
                        pointDeCollecte.SetActive(false);
                }
                break;
            case "Coin":
                pointDeCollecteText.text = "Appuyez sur E pour ramasser la pièce";
                break;
            case "Coins":
                if (!hitGameObject.GetComponent<Coin>().enabled)
                {
                    pointDeCollecte.SetActive(false);
                }
                else
                    pointDeCollecteText.text = "Appuyez sur E pour ramasser les pièces";
                break;
            case "PanneauDeConstruction":
                pointDeCollecteText.text = "Appuyez sur E pour intéragir";
                break;
            case "PersonalChest":
                if (!hitGameObject.GetComponent<PersonalChest>().isOpen)
                {
                    pointDeCollecteText.text = "Appuyez sur E pour ouvrir le coffre";
                }
                break;
            case "Livre":
                pointDeCollecteText.text = "Appuyez sur E pour lire";
                break;
        }
    }
    private void InteractionWithE(RaycastHit hit)
    {
        Transform hitTransform = hit.transform;
        GameObject hitGameObject = hitTransform.gameObject;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hitTransform.CompareTag("Item"))
            {
                playerInteractBehaviour.DoPickUp(hitGameObject.GetComponent<Item>());
            }

            else if (hitTransform.CompareTag("Harvestable"))
            {
                if ((hitGameObject.GetComponent<Harvestable>().tool == Tool.Axe && playerInteractBehaviour.canAxe) || (hitGameObject.GetComponent<Harvestable>().tool == Tool.Pickaxe && playerInteractBehaviour.canPickaxe))
                {
                    playerInteractBehaviour.DoHarvest(hitGameObject.GetComponent<Harvestable>());
                }
            }

            else if (hitTransform.CompareTag("PoigneeDePorte"))
            {
                if(palette.equipmentObject1Item != null && palette.equipmentObject1Item.itemType == ItemType.Key && palette.isEquippedObject1)
                {
                    hitGameObject.GetComponent<Door>().TryToOpenWithKey(palette.equipmentObject1Item);
                }
                else if (palette.equipmentObject2Item != null && palette.equipmentObject2Item.itemType == ItemType.Key && palette.isEquippedObject2)
                {
                    hitGameObject.GetComponent<Door>().TryToOpenWithKey(palette.equipmentObject2Item);
                }
                else
                {
                    hitGameObject.GetComponent<Door>().OpenAndCloseDoor();

                }
            }
            else if (hitTransform.CompareTag("Chest"))
            {
                if (palette.equipmentObject1Item != null && palette.equipmentObject1Item.itemType == ItemType.Key && palette.isEquippedObject1)
                {
                    hitGameObject.GetComponent<Chest>().TryToOpenWithKey(palette.equipmentObject1Item);
                }
                else if (palette.equipmentObject2Item != null && palette.equipmentObject2Item.itemType == ItemType.Key && palette.isEquippedObject2)
                {
                    hitGameObject.GetComponent<Chest>().TryToOpenWithKey(palette.equipmentObject2Item);
                }
                else
                {
                    hitGameObject.GetComponent<Chest>().Open();
                }
            }
            else if (hitTransform.CompareTag("PNJ") && !hit.transform.gameObject.GetComponent<PNJ>().isOnDial && Time.time - hitGameObject.GetComponent<PNJ>().dialogueEndTime > hit.transform.gameObject.GetComponent<PNJ>().inputCooldown)
            {
                hitGameObject.GetComponent<PNJ>().StartDialogue();
            }
            else if (hitTransform.CompareTag("Marchand") && !hit.transform.gameObject.GetComponent<Marchand>().isOnDial && Time.time - hit.transform.gameObject.GetComponent<Marchand>().dialogueEndTime > hitGameObject.GetComponent<Marchand>().inputCooldown)
            {
                hit.transform.gameObject.GetComponent<Marchand>().StartDialogue(hitGameObject.GetComponent<Marchand>().sentences);
            }
            else if (hitTransform.CompareTag("PNJAcheteur") && !hit.transform.gameObject.GetComponent<PNJAcheteur>().isOnDial && Time.time - hit.transform.gameObject.GetComponent<PNJAcheteur>().dialogueEndTime > hitGameObject.GetComponent<PNJAcheteur>().inputCooldown)
            {
                hit.transform.gameObject.GetComponent<PNJAcheteur>().StartDialogue(hitGameObject.GetComponent<PNJAcheteur>().sentences);
            }
            else if (hitTransform.CompareTag("Forgeron") && !hit.transform.gameObject.GetComponent<Forgeron>().isOnDial && Time.time - hit.transform.gameObject.GetComponent<Forgeron>().dialogueEndTime > hitGameObject.GetComponent<Forgeron>().inputCooldown)
            {
                hit.transform.gameObject.GetComponent<Forgeron>().StartDialogue(hitGameObject.GetComponent<Forgeron>().sentences);
            }
            else if (hitTransform.CompareTag("Coin") ||( hitTransform.CompareTag("Coins") && hitGameObject.GetComponent<Coin>().enabled))
            {
                hitGameObject.GetComponent<Coin>().Collect();
            }
            else if (hitTransform.CompareTag("PanneauDeConstruction"))
            {
                hitGameObject.GetComponent<PanneauDeConstruction>().OpenPanel();
            }
            else if (hitTransform.CompareTag("PersonalChest"))
            {
                hitGameObject.GetComponent<PersonalChest>().OpenAndClose();
            }
            else if (hitTransform.CompareTag("Livre"))
            {
                hitGameObject.GetComponent<Livre>().OuvrirFermerLivre();
            }
            else if (hitTransform.CompareTag("CraftingTable"))
            {
                if (hitGameObject.GetComponent<CraftingTable>().isCrafting)
                    hitGameObject.GetComponent<CraftingTable>().OpenCraftingTablePanel(hitGameObject.GetComponent<CraftingTable>().craftingTableParent.recetteDeLObjectCrafting);
                else if (hitGameObject.GetComponent<CraftingTable>().isCooking)
                    hitGameObject.GetComponent<CraftingTable>().OpenCraftingTablePanel(hitGameObject.GetComponent<CraftingTable>().craftingTableParent.recetteDeLObjectCooking);
            }
        }
    }
}