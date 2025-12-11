using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

public class Interact : MonoBehaviour
{
    //[SerializeField] private Palette palette;

    //[SerializeField] private float interactRange = 2.6f;

    //[SerializeField] private GameObject pointDeCollecte;

    //[SerializeField] private TextMeshProUGUI pointDeCollecteText1, pointDeCollecteText2, pointDeCollecteText3;

    //public InteractBehaviour playerInteractBehaviour;

    //[SerializeField] private LayerMask layerMask;

    //[SerializeField] private Image interactIcon;

    //private DeviceType currentDevice;

    //#region PlayerInput
    //[SerializeField] private PlayerInput playerInput;
    //private bool interactPressed = false;
    //#endregion

    //private void Awake()
    //{
    //    if (playerInput == null)
    //        playerInput = GetComponent<PlayerInput>();
    //}

    //void OnEnable()
    //{
    //    playerInput.actions["Interact"].performed += OnInteract;
    //    playerInput.actions["Interact"].canceled += OnInteractCanceled;
    //}
    //void OnDisable()
    //{
    //    playerInput.actions["Interact"].performed -= OnInteract;
    //    playerInput.actions["Interact"].canceled -= OnInteractCanceled;
    //}

    //private void OnInteract(InputAction.CallbackContext context)
    //{
    //    interactPressed = true;
    //}

    //private void OnInteractCanceled(InputAction.CallbackContext context)
    //{
    //    interactPressed = false;
    //}

    //void Update()
    //{
    //    Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red);

    //    if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactRange, layerMask))
    //    {
    //        pointDeCollecte.SetActive(true);
    //        ChangeText(hit);

    //        InteractionWithE(hit);
    //    }
    //    else
    //    {
    //        pointDeCollecte.SetActive(false);
    //    }

    //    DeviceType newDevice =
    //    Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame ? DeviceType.Keyboard :
    //    Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame ? DeviceType.Gamepad :
    //    currentDevice;

    //    if (newDevice != currentDevice)
    //    {
    //        currentDevice = newDevice;
    //    }
    //}

    //private void ChangeText(RaycastHit hit)
    //{
    //    GameObject hitGameObject = hit.transform.gameObject;

    //    // Réactivation par défaut
    //    pointDeCollecteText2.gameObject.SetActive(true);
    //    pointDeCollecteText3.gameObject.SetActive(true);

    //    switch (hit.transform.tag)
    //    {
    //        case "Item":
    //            pointDeCollecteText1.text = "Appuyez sur";
    //            pointDeCollecteText2.text = GetInteractPrefix();
    //            pointDeCollecteText3.text = "pour ramasser";
    //            break;

    //        case "Harvestable":
    //            Harvestable harvestable = hitGameObject.GetComponent<Harvestable>();
    //            if ((harvestable.tool == Tool.Axe && !playerInteractBehaviour.canAxe) ||
    //                (harvestable.tool == Tool.Pickaxe && !playerInteractBehaviour.canPickaxe))
    //            {
    //                pointDeCollecteText1.text = harvestable.tool == Tool.Axe ? "(Hache requise)" : "(Pioche requise)";
    //                pointDeCollecteText2.gameObject.SetActive(false);
    //                pointDeCollecteText3.gameObject.SetActive(false);
    //            }
    //            else
    //            {
    //                pointDeCollecteText1.text = "Appuyez sur";
    //                pointDeCollecteText2.text = GetInteractPrefix();
    //                pointDeCollecteText3.text = "pour détruire";
    //            }
    //            break;

    //        case "PoigneeDePorte":
    //            Door door = hitGameObject.GetComponent<Door>();
    //            if (door.isAnimating)
    //            {
    //                pointDeCollecte.SetActive(false);
    //                return;
    //            }

    //            pointDeCollecteText1.text = "Appuyez sur";
    //            pointDeCollecteText2.text = GetInteractPrefix();
    //            pointDeCollecteText3.text = door.isOpen ? "pour fermer la porte" :
    //                door.isLocked ? "(clé requise)" : "pour ouvrir la porte";
    //            break;

    //        case "Chest":
    //            Chest chest = hitGameObject.GetComponent<Chest>();
    //            //if (!chest.isOpen)
    //            //{
    //            //    pointDeCollecteText1.text = "Appuyez sur";
    //            //    pointDeCollecteText2.text = GetInteractPrefix();
    //            //    pointDeCollecteText3.text = chest.isLocked ? "(clé requise)" : "pour ouvrir le coffre";
    //            //}
    //            //else
    //            //{
    //            //    pointDeCollecte.SetActive(false);
    //            //}
    //            break;

    //        case "PNJ":
    //        case "Marchand":
    //        case "PNJAcheteur":
    //        case "Forgeron":
    //            // Tous les PNJ suivent le même schéma
    //            if (hitGameObject.TryGetComponent(out IDialogue dialogueNPC) &&
    //                !dialogueNPC.IsOnDialogue() &&
    //                Time.time - dialogueNPC.LastDialogueTime() > dialogueNPC.InputCooldown())
    //            {
    //                pointDeCollecteText1.text = "Appuyez sur";
    //                pointDeCollecteText2.text = GetInteractPrefix();
    //                pointDeCollecteText3.text = "pour parler";
    //            }
    //            else
    //            {
    //                pointDeCollecte.SetActive(false);
    //            }
    //            break;

    //        case "Coin":
    //        case "Coins":
    //            pointDeCollecteText1.text = "Appuyez sur";
    //            pointDeCollecteText2.text = GetInteractPrefix();
    //            pointDeCollecteText3.text = hit.transform.CompareTag("Coin") ? "pour ramasser la pièce" : "pour ramasser les pièces";
    //            break;

    //        case "PanneauDeConstruction":
    //            pointDeCollecteText1.text = "Appuyez sur";
    //            pointDeCollecteText2.text = GetInteractPrefix();
    //            pointDeCollecteText3.text = "pour interagir";
    //            break;

    //        case "PersonalChest":
    //            pointDeCollecteText1.text = "Appuyez sur";
    //            pointDeCollecteText2.text = GetInteractPrefix();
    //            pointDeCollecteText3.text = "pour ouvrir le coffre";
    //            break;

    //        case "Livre":
    //            pointDeCollecteText1.text = "Appuyez sur";
    //            pointDeCollecteText2.text = GetInteractPrefix();
    //            pointDeCollecteText3.text = "pour lire";
    //            break;

    //        default:
    //            // Fallback
    //            pointDeCollecteText1.text = "";
    //            pointDeCollecteText2.text = "";
    //            pointDeCollecteText3.text = "";
    //            break;
    //    }
    //}


    //private string GetInteractPrefix()
    //{
    //    InputAction action = playerInput.actions["Interact"];

    //    InputBinding binding;

    //    if (currentDevice == DeviceType.Gamepad)
    //    {
    //        binding = action.bindings.FirstOrDefault(b =>
    //            !string.IsNullOrEmpty(b.effectivePath) &&
    //            b.effectivePath.Contains("<Gamepad>")
    //        );
    //    }
    //    else // Keyboard + Mouse
    //    {
    //        binding = action.bindings.FirstOrDefault(b =>
    //            !string.IsNullOrEmpty(b.effectivePath) &&
    //            (b.effectivePath.Contains("<Keyboard>") || b.effectivePath.Contains("<Mouse>"))
    //        );
    //    }

    //    if (binding != default)
    //    {
    //        interactIcon.sprite = InputIconDatabase.instance.GetIcon(binding.effectivePath);
    //        interactIcon.enabled = true;
    //        return ""; // Pas de texte avant l’icône
    //    }

    //    // fallback
    //    interactIcon.enabled = false;
    //    return" ";
    //}

    //private void InteractionWithE(RaycastHit hit)
    //{
    //    if (!interactPressed)
    //        return;

    //    Transform hitTransform = hit.transform;
    //    GameObject hitGameObject = hitTransform.gameObject;
    //    if (interactPressed)
    //    {
    //        if (hitTransform.CompareTag("Item"))
    //        {
    //            playerInteractBehaviour.DoPickUp(hitGameObject.GetComponent<Item>());
    //        }

    //        else if (hitTransform.CompareTag("Harvestable"))
    //        {
    //            if ((hitGameObject.GetComponent<Harvestable>().tool == Tool.Axe && playerInteractBehaviour.canAxe) || (hitGameObject.GetComponent<Harvestable>().tool == Tool.Pickaxe && playerInteractBehaviour.canPickaxe))
    //            {
    //                playerInteractBehaviour.DoHarvest(hitGameObject.GetComponent<Harvestable>());
    //            }
    //        }

    //        else if (hitTransform.CompareTag("PoigneeDePorte"))
    //        {
    //            if(palette.equipmentObject1Item != null && palette.equipmentObject1Item.itemType == ItemType.Key && palette.isEquippedObject1)
    //            {
    //                hitGameObject.GetComponent<Door>().TryToOpenWithKey(palette.equipmentObject1Item);
    //            }
    //            else if (palette.equipmentObject2Item != null && palette.equipmentObject2Item.itemType == ItemType.Key && palette.isEquippedObject2)
    //            {
    //                hitGameObject.GetComponent<Door>().TryToOpenWithKey(palette.equipmentObject2Item);
    //            }
    //            else
    //            {
    //                hitGameObject.GetComponent<Door>().OpenAndCloseDoor();

    //            }
    //        }
    //        //else if (hitTransform.CompareTag("Chest"))
    //        //{
    //        //    if (palette.equipmentObject1Item != null && palette.equipmentObject1Item.itemType == ItemType.Key && palette.isEquippedObject1)
    //        //    {
    //        //        hitGameObject.GetComponent<Chest>().TryToOpenWithKey(palette.equipmentObject1Item);
    //        //    }
    //        //    else if (palette.equipmentObject2Item != null && palette.equipmentObject2Item.itemType == ItemType.Key && palette.isEquippedObject2)
    //        //    {
    //        //        hitGameObject.GetComponent<Chest>().TryToOpenWithKey(palette.equipmentObject2Item);
    //        //    }
    //        //    else
    //        //    {
    //        //        hitGameObject.GetComponent<Chest>().Open();
    //        //    }
    //        //}
    //        else if (hitTransform.CompareTag("PNJ") && !hit.transform.gameObject.GetComponent<PNJ>().isOnDial && Time.time - hitGameObject.GetComponent<PNJ>().dialogueEndTime > hit.transform.gameObject.GetComponent<PNJ>().inputCooldown)
    //        {
    //            hitGameObject.GetComponent<PNJ>().StartDialogue();
    //        }
    //        else if (hitTransform.CompareTag("Marchand") && !hit.transform.gameObject.GetComponent<Marchand>().isOnDial && Time.time - hit.transform.gameObject.GetComponent<Marchand>().dialogueEndTime > hitGameObject.GetComponent<Marchand>().inputCooldown)
    //        {
    //            hit.transform.gameObject.GetComponent<Marchand>().StartDialogue(hitGameObject.GetComponent<Marchand>().sentences);
    //        }
    //        else if (hitTransform.CompareTag("PNJAcheteur") && !hit.transform.gameObject.GetComponent<PNJAcheteur>().isOnDial && Time.time - hit.transform.gameObject.GetComponent<PNJAcheteur>().dialogueEndTime > hitGameObject.GetComponent<PNJAcheteur>().inputCooldown)
    //        {
    //            hit.transform.gameObject.GetComponent<PNJAcheteur>().StartDialogue(hitGameObject.GetComponent<PNJAcheteur>().sentences);
    //        }
    //        else if (hitTransform.CompareTag("Forgeron") && !hit.transform.gameObject.GetComponent<Forgeron>().isOnDial && Time.time - hit.transform.gameObject.GetComponent<Forgeron>().dialogueEndTime > hitGameObject.GetComponent<Forgeron>().inputCooldown)
    //        {
    //            hit.transform.gameObject.GetComponent<Forgeron>().StartDialogue(hitGameObject.GetComponent<Forgeron>().sentences);
    //        }
    //        else if (hitTransform.CompareTag("Coin") ||( hitTransform.CompareTag("Coins") && hitGameObject.GetComponent<Coin>().enabled))
    //        {
    //            hitGameObject.GetComponent<Coin>().Collect();
    //        }
    //        else if (hitTransform.CompareTag("PanneauDeConstruction"))
    //        {
    //            hitGameObject.GetComponent<PanneauDeConstruction>().OpenPanel();
    //        }
    //        else if (hitTransform.CompareTag("PersonalChest"))
    //        {
    //            hitGameObject.GetComponent<PersonalChest>().OpenAndClose();
    //        }
    //        else if (hitTransform.CompareTag("Livre"))
    //        {
    //            hitGameObject.GetComponent<Livre>().OuvrirFermerLivre();
    //        }
    //        else if (hitTransform.CompareTag("CraftingTable"))
    //        {
    //            if (hitGameObject.GetComponent<CraftingTable>().isCrafting)
    //                hitGameObject.GetComponent<CraftingTable>().OpenCraftingTablePanel(/*hitGameObject.GetComponent<CraftingTable>().craftingTableParent.recetteDeLObjectCrafting*/);
    //            else if (hitGameObject.GetComponent<CraftingTable>().isCooking)
    //                hitGameObject.GetComponent<CraftingTable>().OpenCraftingTablePanel(/*hitGameObject.GetComponent<CraftingTable>().craftingTableParent.recetteDeLObjectCooking*/);
    //        }
    //    }
    //    interactPressed = false;
    //}
}