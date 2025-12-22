using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PNJAcheteur : PNJParent
{
    [Header("Pourcentage de Rachat")]
    [SerializeField] private float pourcentageDeRachat = 0.9f;

    [Header("Reputation")]
    [SerializeField] private int pointMinReputationPourGain;
    [SerializeField] private float pourcentageGain;
    [SerializeField] private int pointmaxReputationPourPerte;
    [SerializeField] private float pourcentagePerte;

    public override void OnInteract(PlayerInteractor player)
    {
        if (isOnDial && Time.time - dialogueStartTime > inputCooldown && !animatorPanelProduits.GetBool("PanelIsOpen"))
        {
            if (!DialogueManager.instance.SkipOrFinish(currentSpeaker) && !DialogueManager.instance.inDelay)
                StartDialogue(sentences);
        }
        else
        {
            StartDialogue(sentences);
            SetTargeted(false, playerTransform);
        }
    }
    // GESTION DU DIALOGUE
    public void StartDialogue(List<DialogueResponse> sentence)
    {
        if (index == 0 && leghthSentences == sentences.Count)
        {
            if (VerifIfEmpty())
            {
                sentence.Add(
                    new DialogueResponse
                    {
                        pnjDialogues = new string[] { "Oh mais je vois que vous n'avez rien à vendre. Revenez me voir lorsque vous aurez quelque chose pour moi !" },
                        playerResponses = new string[] { "D'accord, à une prochaine fois !" }
                    }
                    );
            }
            else
            {
                sentence.Add(
                new DialogueResponse
                {
                    pnjDialogues = new string[] { },
                    playerResponses = new string[] { "Proposez moi vos prix !" }
                }
                );
            }
        }
        if (!isOnDial)
        {
            StartCoroutine(RotateTowardsPlayer());
            moveBehaviour.StopPlayer();
            isOnDial = true;

            BasicBehaviour behaviourManager = playerTransform.GetComponent<BasicBehaviour>();
            behaviourManager.GetAnim.SetFloat("Speed", 0f, 0f, Time.deltaTime);

            DialogueManager.instance.textName.text = namePNJ;

            index = 0;
            dialogueStartTime = Time.time; // Enregistrer le temps de début du dialogue
            currentDialogue = sentence;

            var uiManager = UIManager.instance;
            if (uiManager != null)
            {
                uiManager.HandlePanelOpened();
            }
        }
        else if (!animatorPanelProduits.GetBool("PanelIsOpen") && index >= sentences.Count)
        {
            RefreshProduits();
            if (!VerifIfEmpty())
            {
                OpenProduitsPanel();
            }
            EndDiscussion();
            return;
        }

        var dialogueGroup = currentDialogue[index];

        // Affiche le dialogue PNJ ou la réponse du joueur selon l'index
        if (sentenceIndex < dialogueGroup.pnjDialogues.Length)
        {
            if (!firstDialoguePnjDone)
            {
                DialogueManager.instance.ShowLine(dialogueGroup.pnjDialogues[sentenceIndex], DialogueManager.Speaker.PNJ, 0.5f);
                DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePanel);
                firstDialoguePnjDone = true;
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
                DialogueManager.instance.ShowLine(dialogueGroup.playerResponses[playerIndex], DialogueManager.Speaker.Player, 0.5f);
                firstDialoguePlayerDone = true;
                DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePlayerPanel);
            }
            else
                DialogueManager.instance.ShowLine(dialogueGroup.playerResponses[playerIndex], DialogueManager.Speaker.Player);
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

    // GESTION DES PRODUITS
    private void RefreshProduits()
    {
        // CLEAR PARENTS PRODUITS
        foreach (Transform child in parentsProduits.transform)
        {
            Destroy(child.gameObject);
        }

        // INVENTORY
        foreach (ItemInInventory produit in Inventory.instance.GetContent())
        {
            VerifItemData(produit.itemData, Vendre);
        }

        // OBJECTS
        foreach (ItemInInventory produit in Palette.instance.objects)
        {
            VerifItemData(produit.itemData, VendreObjects);
        }

        // WEAPONS
        foreach (ItemInInventory produit in Palette.instance.weapons)
        {
            VerifItemData(produit.itemData, VendreWeapons);
        }

        // EQUIPMENT

        VerifItemData(Equipment.instance.equipmentHeadItem, VendreEquipment);
        VerifItemData(Equipment.instance.equipmentChestItem, VendreEquipment);
        VerifItemData(Equipment.instance.equipmentHandsItem, VendreEquipment);
        VerifItemData(Equipment.instance.equipmentLegsItem, VendreEquipment);
        VerifItemData(Equipment.instance.equipmentFeetItem, VendreEquipment);
        if (VerifIfEmpty())
        {
            EndCommerce();
        }
    }

    private void VerifItemData(ItemData item, Action<ItemData> methode)
    {
        if (item != null && item.prix > 0)
        {
            GameObject produitItem = Instantiate(produitItemPrefab, parentsProduits.transform);
            Transform childName = produitItem.transform.GetChild(0); // Correct usage of GetChild
            if (childName.TryGetComponent<TextMeshProUGUI>(out var nameText))
            {
                nameText.text = item.itemName; // Assign the name text
            }
            Transform childIcone = produitItem.transform.GetChild(1); // Correct usage of GetChild
            if (childIcone.TryGetComponent<Image>(out var spriteRenderer))
            {
                spriteRenderer.sprite = item.visual; // Assign the sprite
            }
            // PRIX
            Transform childPrix = produitItem.transform.GetChild(2); // Correct usage of GetChild
            if (childPrix.TryGetComponent<TextMeshProUGUI>(out var prixText))
            {
                if(PlayerStats.instance.reputationData.reputationPoints == 0)
                    prixText.text = Mathf.RoundToInt(item.prix * pourcentageDeRachat).ToString(); 

                else if (PlayerStats.instance.reputationData.reputationPoints <= pointmaxReputationPourPerte)
                        prixText.text = Mathf.RoundToInt(item.prix * (pourcentageDeRachat-pourcentagePerte)).ToString(); 

                else if (PlayerStats.instance.reputationData.reputationPoints >= pointmaxReputationPourPerte)
                        prixText.text = Mathf.RoundToInt(item.prix * (pourcentageDeRachat+pourcentageGain)).ToString(); 

            }
            // BUTTON
            Transform childButton = produitItem.transform.GetChild(3); // Correct usage of GetChild
            if (childButton.TryGetComponent<Button>(out var button))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => methode(item));
                if (button.gameObject.TryGetComponent<UISelectable>(out var uiSelectable))
                {
                    navManager.elements.Add(uiSelectable);
                }
            }
        }
    }
    private void Vendre(ItemData produit)
    {
        PlayerStats.instance.goldAmount += Mathf.RoundToInt(produit.prix * pourcentageDeRachat);
        PlayerStats.instance.UpdateGoldText();
        Inventory.instance.RemoveItem(produit);
        RefreshProduits();
    }
    private void VendreObjects(ItemData produit)
    {
        PlayerStats.instance.goldAmount += Mathf.RoundToInt(produit.prix * pourcentageDeRachat);
        PlayerStats.instance.UpdateGoldText();
        if (produit == Palette.instance.equipmentObject1Item)
            Palette.instance.DesequipObject(1);
        else if (produit == Palette.instance.equipmentObject2Item)
            Palette.instance.DesequipObject(2);
        Inventory.instance.RemoveItem(produit);
        RefreshProduits();
    }
    private void VendreWeapons(ItemData produit)
    {
        PlayerStats.instance.goldAmount += Mathf.RoundToInt(produit.prix * pourcentageDeRachat);
        PlayerStats.instance.UpdateGoldText();
        if (produit == Palette.instance.equipmentWeapon1Item)
            Palette.instance.DesequipWeapon(1);
        else if (produit == Palette.instance.equipmentWeapon2Item)
            Palette.instance.DesequipWeapon(2);
        Inventory.instance.RemoveItem(produit);
        RefreshProduits();
    }

    private void VendreEquipment(ItemData produit)
    {
        PlayerStats.instance.goldAmount += Mathf.RoundToInt(produit.prix * pourcentageDeRachat);
        PlayerStats.instance.UpdateGoldText();
        Equipment.instance.DesequipEquipment(produit.equipmentType);
        Inventory.instance.RemoveItem(produit);
        RefreshProduits();
    }

    private bool VerifIfEmpty()
    {
        return Inventory.instance.GetContent().Count == 0 && 
            Palette.instance.equipmentObject1Item == null && Palette.instance.equipmentObject2Item == null &&
            Palette.instance.equipmentWeapon1Item == null && Palette.instance.equipmentWeapon2Item == null && 
            Equipment.instance.equipmentHeadItem == null && Equipment.instance.equipmentChestItem == null &&
            Equipment.instance.equipmentHandsItem == null && Equipment.instance.equipmentLegsItem == null &&
            Equipment.instance.equipmentFeetItem == null;
    }    
}
