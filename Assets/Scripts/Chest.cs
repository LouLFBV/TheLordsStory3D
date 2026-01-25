using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class Chest : InteractableBase
{
    #region Champs
    [Header("References")]
    [SerializeField] private Palette palette;
    [SerializeField] private EquipmentLibrary equipmentLibrary;

    [Header("Description Panel")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image objectImage;

    [Header("Chest Parts")]
    [SerializeField] private GameObject topChest;
    [SerializeField] private Rigidbody topLock;
    [SerializeField] private Rigidbody bottomLock;

    [Header("Chest Settings")]
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private Vector3 openEulerAngles = new Vector3(0, 0, 90);
    [SerializeField] private bool isLocked = false;
    [SerializeField] private ItemData keyItem;

    [Header("Audio")]
    [SerializeField] private AudioSource openSound;
    [SerializeField] private AudioSource lockedSound;
    [SerializeField] private AudioSource unlockSound;

    [Header("Reward")]
    [SerializeField] private ItemData rewardItem;
    [SerializeField] private int rewardAmount = 1;

    [Header("Gold Reward")]
    [SerializeField] private int goldAmount = 0;
    [SerializeField] private GameObject goldVisual;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;
    private bool isAnimating = false;
    private bool isAlive = true;

    private Animator playerAnimator;
    private PlayerInput playerInput;

    #endregion 
    // -------------------------------------------------------
    // INITIALISATION
    // -------------------------------------------------------

    private void Start()
    {

        // --- Chest Setup ---
        closedRotation = topChest.transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(openEulerAngles);

        if (TryGetComponent<WorldObjectID>(out var worldID))
        {
            if (WorldStateManager.Instance.IsCollected(worldID.uniqueID))
            {
                Destroy(gameObject);
                return;
            }
        }
        Debug.Log("<color=cyan>[CHEST] Initialisation du coffre…</color>");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
            playerAnimator = player.GetComponent<Animator>();
            Debug.Log("[CHEST] PlayerInput trouvé !");
        }
        else
        {
            Debug.LogError("[CHEST] Player introuvable !");
        }


        if (palette == null)
            palette = Palette.instance;
        if (equipmentLibrary == null)
            equipmentLibrary = GameObject.FindWithTag("GameManager").GetComponent<EquipmentLibrary>();

        if (isLocked)
            objectType = InteractableObjectType.Key;        
        else
            objectType = InteractableObjectType.Chest;
        interactUI.SetInteractable(this);
    }


    private void ActiveCancel()
    {
        if (playerInput != null)
        {
            var cancel = playerInput.actions["Cancel"];

            if (cancel == null)
            {
                Debug.LogError("[CHEST] Action 'Cancel' introuvable dans PlayerInput !");
            }
            else
            {
                cancel.performed += OnCancel;
                cancel.Enable();
                Debug.Log("<color=lime>[CHEST] Cancel correctement bindé.</color>");
            }
        }
        else
        {
            Debug.LogError("[CHEST] PlayerInput est NULL !");
        }
    }
    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Cancel"].performed -= OnCancel;
        }
    }


    private void OnDestroy()
    {
        isAlive = false;

        if (playerInput != null)
        {
            playerInput.actions["Cancel"].performed -= OnCancel;
        }
    }

    // -------------------------------------------------------
    // INPUT : Cancel
    // -------------------------------------------------------

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!isAlive || descriptionPanel == null) return;

        if (descriptionPanel.activeSelf)
        {
            descriptionPanel.SetActive(false);
            playerInput.actions["Cancel"].performed -= OnCancel;
            playerInput.actions["Cancel"].Disable();
        }
    }


    // -------------------------------------------------------
    // INTERACTION
    // -------------------------------------------------------

    public override void OnInteract(PlayerInteractor player)
    {
        if (isAnimating || isOpen) return;
        TryOpenChest();
    }


    private void TryOpenChest()
    {
        if (isAnimating) return;

        // Déverrouillage
        if (isLocked && TryUnlockWithEquippedKey())
        {
            unlockSound.Play();
            isLocked = false;
        }

        if (isLocked)
        {
            lockedSound.Play();
            return;
        }

        // OUVERTURE
        if (!isOpen)
        {
            StartCoroutine(OpenChest());
        }
    }

    private bool TryUnlockWithEquippedKey()
    {
        ItemData equipped1 = palette.equipmentObject1Item;
        ItemData equipped2 = palette.equipmentObject2Item;

        if (equipped1 == keyItem && palette.isEquippedObject1)
        {
            RemoveEquippedKey(1, equipped1);
            return true;
        }

        if (equipped2 == keyItem && palette.isEquippedObject2)
        {
            RemoveEquippedKey(2, equipped2);
            return true;
        }

        return false;
    }

    private void RemoveEquippedKey(int slot, ItemData equipped)
    {
        EquipmentLibraryItem libItem = equipmentLibrary.content
            .Where(x => x.itemData == equipped)
            .FirstOrDefault();

        libItem?.itemPrefab.SetActive(false);

        palette.RemoveObject(slot);
        palette.isEquippedObject1 = false;
        palette.isEquippedObject2 = false;
        palette.UpdateImageSeleted();
        playerAnimator.SetBool("CarryingConsumable", false);
    }

    // -------------------------------------------------------
    // OPEN ANIMATION
    // -------------------------------------------------------

    private IEnumerator OpenChest()
    {
        isAnimating = true;
        isOpen = true;

        interactUI.Hide();
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");
        openSound.Play();

        // Reward immédiat
        if (rewardItem == null && goldAmount > 0)
        {
            goldVisual.GetComponent<Coin>().goldAmount = goldAmount;
            goldVisual.SetActive(true);
        }
        else if (rewardItem != null)
        {
            ShowDescriptionPanel();
        }

        // Unlocking physics
        topLock.isKinematic = false;
        bottomLock.isKinematic = false;

        // Animation
        while (Quaternion.Angle(topChest.transform.rotation, openRotation) > 0.1f)
        {
            topChest.transform.rotation = Quaternion.Slerp(
                topChest.transform.rotation,
                openRotation,
                Time.deltaTime * rotationSpeed
            );
            yield return null;
        }

        topChest.transform.rotation = openRotation;

        if (TryGetComponent<WorldObjectID>(out var worldID))
        {
            WorldStateManager.Instance.RegisterCollectedObject(worldID.uniqueID);
        }
    }



    // -------------------------------------------------------
    // REWARD UI
    // -------------------------------------------------------

    private void ShowDescriptionPanel()
    {
        descriptionPanel.SetActive(true);
        ActiveCancel();
        nameText.text = rewardItem.itemName;
        objectImage.sprite = rewardItem.visual;

        if (rewardAmount > 1)
        {
            amountText.text = $"x{rewardAmount}";
            for (int i = 0; i < rewardAmount; i++)
                Inventory.instance.AddItem(rewardItem);
        }
        else
        {
            amountText.text = "";
            Inventory.instance.AddItem(rewardItem);
        }
    }
}
