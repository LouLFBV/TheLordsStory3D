using System.Collections;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Palette palette; // Référence à la palette pour vérifier les items
    [SerializeField] private EquipmentLibrary equipmentLibrary; // Référence à l'item d'équipement si nécessaire

    [Header("Description d'objet")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image imageObject;

    [Header("Chest Parts")]
    [SerializeField] private GameObject topChest;
    [SerializeField] private Rigidbody topLock;
    [SerializeField] private Rigidbody bottomLock;
    [SerializeField] private GameObject bottomChest;

    [Header("Chest Settings")]
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private Vector3 openEulerAngles = new Vector3(0, 0, 90);
    public bool isLocked = false;
    [SerializeField] private ItemData keyItem; 

    [SerializeField] private AudioSource lockedChestSound;

    [SerializeField] private AudioSource Opensound;

    [SerializeField] private AudioSource unlockChestSound;

    [Header("ItemR eward")]
    [SerializeField] private ItemData rewardItemData;
    [SerializeField] private int amountOfItem;

    [Header("Gold Reward")]
    [SerializeField] private int amountOfGold;
    [SerializeField] private GameObject goldVisual;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    public bool isOpen = false;
    private bool isAnimating = false;
    private bool destroyed = false;
    private Animator animator;
    private UIManager uiManager;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    private void Start()
    {
        closedRotation = topChest.transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(openEulerAngles);
        bottomChest.GetComponent<Harvestable>().enabled = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            animator = player.GetComponent<Animator>();
        }

        uiManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIManager>();
        uiManager.AddPanel(descriptionPanel);
    }
    private void Update()
    {
        if (!destroyed && !isAnimating)
        {
            BecomeHarvestable();
        }
        else if (bottomChest == null && topChest != null)
        {
            Destroy(topChest);
        }

        if(descriptionPanel.activeSelf && controls.UI.Cancel.triggered)
        {
            descriptionPanel.SetActive(false);
        }
    }

    public void Open()
    {
        if (!isOpen && !isAnimating && !isLocked)
        {
            StartCoroutine(OpenChest());
        }
        else if (isLocked)
        {
            lockedChestSound.PlayOneShot(lockedChestSound.clip);
        }
    }

    private IEnumerator OpenChest()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        isAnimating = true;
        isOpen = true;
        Opensound.PlayOneShot(Opensound.clip);
        if (rewardItemData == null && amountOfGold > 0)
        {
            goldVisual.GetComponent<Coin>().goldAmount = amountOfGold;
            goldVisual.SetActive(true);
        }
        else
        {
            ActiveDescriptionPanel();
        }
        topLock.isKinematic = false;
        bottomLock.isKinematic = false;
        while (Quaternion.Angle(topChest.transform.rotation, openRotation) > 0.1f)
        {
            topChest.transform.rotation = Quaternion.Slerp(
                topChest.transform.rotation,
                openRotation,
                Time.deltaTime * rotationSpeed
            );
            yield return null;
        }

        // S’assure que la rotation est précise à la fin
        topChest.transform.rotation = openRotation;


        isAnimating = false;
    }

    public void BecomeHarvestable()
    {
        if (isOpen)
        {
            destroyed = true;
            bottomChest.layer = LayerMask.NameToLayer("Harvestable");
            bottomChest.tag = "Harvestable";
            bottomChest.GetComponent<Harvestable>().enabled = true;
        }
    }

    public void TryToOpenWithKey(ItemData key)
    {
        if (equipmentLibrary == null)
        {
             equipmentLibrary = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EquipmentLibrary>();
        }
        if (palette == null)
            palette = Palette.instance;
        if (isLocked && key != null && key == keyItem)
        {
            ItemInInventory itemInInventory = palette.objects.Where(x => x.itemData == key).FirstOrDefault();
            if (itemInInventory != null && palette.isEquippedObject1)
            {
                EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.content.Where(x => x.itemData == palette.equipmentObject1Item).FirstOrDefault();
                if (equipmentLibraryItem1 != null)
                {
                    equipmentLibraryItem1.itemPrefab.SetActive(false);
                }
                palette.RemoveObject(1);
                palette.isEquippedObject1 = false;
            }
            else if (itemInInventory != null && palette.isEquippedObject2)
            {
                EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.content.Where(x => x.itemData == palette.equipmentObject2Item).First();
                if (equipmentLibraryItem2 != null)
                {
                    equipmentLibraryItem2.itemPrefab.SetActive(false);
                }
                palette.RemoveObject(2);
                palette.isEquippedObject2 = false;
            }
            palette.UpdateImageSeleted();
            animator.SetBool("CarryingConsumable", false);
            isLocked = false;
            unlockChestSound.PlayOneShot(unlockChestSound.clip);
            Open();
        }
        else if (isLocked)
        {
            lockedChestSound.PlayOneShot(lockedChestSound.clip);
        }
    }

    private void ActiveDescriptionPanel()
    {
        descriptionPanel.SetActive(true);
        nameText.text = rewardItemData.itemName;
        imageObject.sprite = rewardItemData.visual;
        if (amountOfItem > 1)
        {
            amountText.text = "x" + amountOfItem.ToString();
            for (int i = 0; i < amountOfItem; i++)
            {
                Inventory.instance.AddItem(rewardItemData);
            }
        }
        else
        {
            amountText.text = "";
            Inventory.instance.AddItem(rewardItemData);
        }
    }
}
