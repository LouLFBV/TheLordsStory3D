using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class Marchand : InteractableBase
{
    private UIManager uIManager;
    [Header("Panel")]
    [SerializeField] private GameObject parentsProduits;
    [SerializeField] private GameObject panelProduits;
    [SerializeField] private ItemData[] produits;
    [SerializeField] private GameObject produitItemPrefab;
    [SerializeField] private Animator animatorPanelProduits;
    private DialogueManager.Speaker currentSpeaker;
    [SerializeField] private GameObject isActive;

    [Header("PNJ")]
    public string namePNJ;
    public DialogueResponse[] sentences;
    public bool isOnDial;
    private int index = 0;
    private int sentenceIndex = 0;
    private DialogueResponse[] currentDialogue; // tableau actif
    private bool firstDialoguePlayerDone = false, firstDialoguePnjDone = false;
    private Transform playerTransform;
    private MoveBehaviour moveBehaviour;
    private Animator animator;


    [HideInInspector] public float inputCooldown = 0.2f; // Temps d'attente après lancement du dialogue
    [HideInInspector] public float dialogueStartTime, dialogueEndTime;


    [SerializeField] private UINavigationManager navManager;

    private void Start()
    {
       
        animator = GetComponent<Animator>();
        uIManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIManager>();
        uIManager.AddPanel(isActive);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        moveBehaviour = playerTransform.GetComponent<MoveBehaviour>();
    }
    public override void OnInteract(PlayerInteractor player)
    {
        Debug.Log("Interact Marchand");
        if (isOnDial && Time.time - dialogueStartTime > inputCooldown && !animatorPanelProduits.GetBool("PanelIsOpen"))
        {
            if (!DialogueManager.instance.SkipOrFinish(currentSpeaker) && !DialogueManager.instance.inDelay)
                StartDialogue(sentences);
        }
        else if (!isOnDial)
        {
            StartDialogue(sentences);
        }
    }

    // GESTION DU DIALOGUE
    public void StartDialogue(DialogueResponse[] sentence)
    {
        if (!isOnDial)
        {
            var uiManager = UIManager.instance;
            if (uiManager != null)
            {
                uiManager.HandlePanelOpened();
            }
            isOnDial = true;

            BasicBehaviour behaviourManager = playerTransform.GetComponent<BasicBehaviour>();
            behaviourManager.GetAnim.SetFloat("Speed", 0f, 0f, Time.deltaTime);

            DialogueManager.instance.textName.text = namePNJ;

            index = 0;
            dialogueStartTime = Time.time; // Enregistrer le temps de début du dialogue
            currentDialogue = sentence;
        }
        if (index >= currentDialogue.Length && !animatorPanelProduits.GetBool("PanelIsOpen"))
        {
            OpenProduitsPanel();
            EndDiscussion();
            return;
        }


        var dialogueGroup = currentDialogue[index];

        // Affiche le dialogue PNJ ou la réponse du joueur selon l'index
        if (sentenceIndex < dialogueGroup.pnjDialogues.Length)
        {
            if (!firstDialoguePnjDone)
            {
                DialogueManager.instance.ShowLine(dialogueGroup.pnjDialogues[sentenceIndex], DialogueManager.Speaker.PNJ, 0.75f);
                firstDialoguePnjDone = true;
                DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePanel);
            }
            else
                DialogueManager.instance.ShowLine(dialogueGroup.pnjDialogues[sentenceIndex], DialogueManager.Speaker.PNJ);
            animator.SetBool("isTalking", true);
            currentSpeaker = DialogueManager.Speaker.PNJ;
        }
        else
        {
            int playerIndex = sentenceIndex - dialogueGroup.pnjDialogues.Length;
            if (!firstDialoguePlayerDone)
            {
                DialogueManager.instance.ShowLine(dialogueGroup.playerResponses[playerIndex], DialogueManager.Speaker.Player, 0.75f);
                firstDialoguePlayerDone = true;
                DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePlayerPanel);
            }
            else
                DialogueManager.instance.ShowLine(dialogueGroup.playerResponses[sentenceIndex], DialogueManager.Speaker.Player);
            animator.SetBool("isTalking", false);
            currentSpeaker = DialogueManager.Speaker.Player;
        }
        sentenceIndex++;

        // Si on a fini toutes les lignes du groupe, passe au groupe suivant
        if (sentenceIndex >= dialogueGroup.pnjDialogues.Length + dialogueGroup.playerResponses.Length)
        {
            sentenceIndex = 0;
            index++;
        }
    }

    public void EndCommerce()
    {
        isOnDial = false;
        dialogueEndTime = Time.time;
        animatorPanelProduits.SetBool("PanelIsOpen", false);
        isActive.SetActive(false);
        var uiManager = UIManager.instance;
        if (uiManager != null)
        {
            uiManager.HandlePanelClosed();
        }
        if (navManager != null)
        {
            navManager.onCancel = null;
        }
    }
    public void EndDiscussion()
    {
        Debug.Log("EndDiscussion Marchand");
        firstDialoguePnjDone = false;
        firstDialoguePlayerDone = false;
        animator.SetBool("isTalking", false);
        dialogueEndTime = Time.time;
        index = 0;

        if (DialogueManager.instance.dialoguePanel.transform.localScale.y > 0)
            DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePanel);

        if (DialogueManager.instance.dialoguePlayerPanel.transform.localScale.y > 0)
            DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePlayerPanel);
    }

    public void OpenProduitsPanel()
    {
        animatorPanelProduits.SetBool("PanelIsOpen", true);
        RefreshProduits();
        isActive.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (navManager != null)
        {
            navManager.onCancel = EndCommerce;
        }
    }
    // GESTION DES PRODUITS
    private void RefreshProduits()
    {
        foreach (Transform child in parentsProduits.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (ItemData produit in produits)
        {
            GameObject produitItem = Instantiate(produitItemPrefab, parentsProduits.transform);

            Transform childName = produitItem.transform.GetChild(0); // Correct usage of GetChild
            if (childName.TryGetComponent<TextMeshProUGUI>(out var nameText))
            {
                nameText.text = produit.itemName; // Assign the name text
            }


            Transform childIcone = produitItem.transform.GetChild(1); // Correct usage of GetChild
            if (childIcone.TryGetComponent<Image>(out var spriteRenderer))
            {
                spriteRenderer.sprite = produit.visual; // Assign the sprite
            }

            Transform childDescription = produitItem.transform.GetChild(2); // Correct usage of GetChild
            if (childDescription.TryGetComponent<TextMeshProUGUI>(out var descriptionText))
            {
                descriptionText.text = produit.description; // Assign the description text
            }

            Transform childButton = produitItem.transform.GetChild(3); // Correct usage of GetChild
            if (childButton.TryGetComponent<Button>(out var button))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { Acheter(produit); });
                VerfifButtonAcheter(produit, button); // Check if the button should be interactable

                if(button.TryGetComponent<UISelectable>(out var uiSelectable))
                {
                    navManager.elements.Add(uiSelectable);
                }
            }

            Transform childPrix = produitItem.transform.GetChild(4).GetChild(0); // Correct usage of GetChild
            if (childPrix.TryGetComponent<TextMeshProUGUI>( out var prixText))
            {
                prixText.text = "Prix : " + produit.prix.ToString(); // Assign the price text
            }
        }
    }

    private void Acheter(ItemData produit)
    {
        if (produit.prix <= PlayerStats.instance.goldAmount)
        {
            PlayerStats.instance.goldAmount -= produit.prix;
            PlayerStats.instance.UpdateGoldText();
            Inventory.instance.AddItem(produit);
            RefreshProduits();
        }
    }

    private void VerfifButtonAcheter(ItemData produit, Button buyButton)
    {
        Image buttonImage = buyButton.GetComponent<Image>();
        if (produit.prix <= PlayerStats.instance.goldAmount && VerifInInventoryAndPalette(produit))
        {
            buttonImage.color = Color.green; // Set button color to white if affordable
            buyButton.interactable = true;
        }
        else
        {
            buttonImage.color = Color.red; // Set button color to red if not affordable
            buyButton.interactable = false;
        }
    }

    private bool VerifInInventoryAndPalette(ItemData produit)
    {
        foreach (ItemInInventory item in Inventory.instance.GetContent())
        {
            if ((item.itemData.itemType == ItemType.Equipment  || item.itemData.itemType == ItemType.Key || item.itemData.itemType == ItemType.QuestItem) && item.itemData == produit)
            {
                return false; // Item is already in the inventory
            }
        }
        if (Palette.instance.equipmentWeapon1Item == produit || Palette.instance.equipmentWeapon2Item == produit)
        {
            return false; // Item is already equipped in weapon slot 1
        }
        else if (Palette.instance.equipmentObject1Item == produit  && (Palette.instance.equipmentObject1Item.itemType == ItemType.Equipment || 
            Palette.instance.equipmentObject1Item.itemType == ItemType.Key ||
            Palette.instance.equipmentObject1Item.itemType == ItemType.QuestItem) 
            ||
            (Palette.instance.equipmentObject2Item == produit && (Palette.instance.equipmentObject2Item.itemType == ItemType.Equipment ||
            Palette.instance.equipmentObject2Item.itemType == ItemType.Key ||
            Palette.instance.equipmentObject2Item.itemType == ItemType.QuestItem)))
        {
            return false; // Item is already equipped in armor slots
        }
        return true; // Item is not in the inventory
    }    
}
