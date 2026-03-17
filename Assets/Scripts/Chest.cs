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
    [SerializeField] private int niveauDeVerrouillage = 0;

    [Header("Audio")]
    [SerializeField] private AudioSource openSound;
    [SerializeField] private AudioSource lockedSound;
    [SerializeField] private AudioSource unlockSound;
    [SerializeField] private AudioSource unlockFailSound;

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

        
        Debug.Log("<color=cyan>[CHEST] Initialisation du coffre…</color>");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
            Debug.Log("[CHEST] PlayerInput trouvé !");
        }
        else
        {
            Debug.LogError("[CHEST] Player introuvable !");
        }

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
        if (WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.OnWorldStateLoaded -= Apply;
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

    private void OnEnable()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnWorldStateLoaded += Apply;
    }

    public void Apply()
    {
        if (TryGetComponent<WorldObjectID>(out var worldID))
        {
            if (WorldStateManager.Instance.IsCollected(worldID.UniqueID))
            {
                Destroy(gameObject);
                return;
            }
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
            PlayerController.Instance.StateMachine.ChangeState(PlayerStateType.Idle);
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

        if (isLocked && InventorySystem.instance.KeyIsInInventory(keyItem))
            TryToOpenWithKey(keyItem);
        else if (!isOpen && !isLocked)
            StartCoroutine(OpenChest());
        else if (isLocked)
            lockedSound.Play();
    }

    public void TryToOpenWithKey(ItemData key)
    {
        if (!isLocked)
        {
            StartCoroutine(OpenChest());
            return;
        }
        // Cas 1 : la bonne clé est utilisée
        else if (key == keyItem)
        {
            InventorySystem.instance.RemoveItem(key);
            StartCoroutine(OpenChest());
            return;
        }
        // Cas 2 : tentative de crochetage avec une clé "improvisée"
        else if (key.attackPoints > 0)
        {
            float chanceDeReussite = Mathf.Clamp01((float)key.attackPoints / (niveauDeVerrouillage + 1));
            float tirage = Random.value;

            Debug.Log($"Chance de réussite : {chanceDeReussite}, tirage : {tirage}");

            if (tirage <= chanceDeReussite)
            {
                StartCoroutine(OpenChest());
            }
            else
            {
                unlockFailSound.PlayOneShot(unlockFailSound.clip);
                if (PlayerStats.instance.reputationData.reputationPoints > -10)
                    PlayerStats.instance.reputationData.reputationPoints -= 1;
            }
            Inventory.instance.RemoveItem(key);
        }
        else
        {
            lockedSound.Play();
        }
    }


    // -------------------------------------------------------
    // OPEN ANIMATION
    // -------------------------------------------------------

    private IEnumerator OpenChest()
    {
        isLocked = false;
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
            WorldStateManager.Instance.RegisterCollectedObject(worldID.UniqueID);
        }
    }



    // -------------------------------------------------------
    // REWARD UI
    // -------------------------------------------------------

    private void ShowDescriptionPanel()
    {
        descriptionPanel.SetActive(true);
        PlayerController.Instance.StateMachine.ChangeState(PlayerStateType.UI);
        ActiveCancel();
        nameText.text = rewardItem.itemName;
        objectImage.sprite = rewardItem.visual;

        if (rewardAmount > 1)
        {
            amountText.text = $"x{rewardAmount}";
            for (int i = 0; i < rewardAmount; i++)
                InventorySystem.instance.AddItem(rewardItem);
        }
        else
        {
            amountText.text = "";
            InventorySystem.instance.AddItem(rewardItem);
        }
    }
}
