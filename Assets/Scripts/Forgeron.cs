using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Forgeron : PNJParent
{
    [Header("Upgrade")]
    [SerializeField] private int prixDeLAmelioration = 10;
    [SerializeField] private int upGradeAmount = 10;
    [SerializeField] private int upGradeAmountPourcentage = 5;

    public override void OnInteract(PlayerInteractor player)
    {
        if (isOnDial && Time.time - dialogueStartTime > inputCooldown && !animatorPanelProduits.GetBool("PanelIsOpen"))
        {
            if (!DialogueManager.instance.SkipOrFinish(currentSpeaker) && !DialogueManager.instance.inDelay)
                StartDialogue(sentences);
        }
        else
        {
            SetTargeted(false, playerTransform);
            StartDialogue(sentences);
        }
    }

    // GESTION DU DIALOGUE
    public void StartDialogue(List<DialogueResponse> sentence = null)
    {
        if (index == 0 && leghthSentences == sentences.Count)
        {
            if (VerifIfEmpty())
            {
                sentence.Add(
                new DialogueResponse
                {
                    pnjDialogues = new string[] { "Hmm je vois que tu n'as pas d'armes. Reviens me voir lorsque tu en auras !" },
                    playerResponses = new string[] { "Pas de soucis, à bientôt !" }
                }
                );
            }
            else
            {
                sentence.Add(
                new DialogueResponse
                {
                    pnjDialogues = new string[] { "Montre moi ce que tu as !" },
                    playerResponses = new string[] {}
                }
                );
            }
        }

        if (!isOnDial)
        {
            StartCoroutine(RotateTowardsPlayer());
            moveBehaviour.StopPlayer();
            isOnDial = true;
            animator.SetBool("isTalking", true);

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
        if (index >= currentDialogue.Count && !animatorPanelProduits.GetBool("PanelIsOpen"))
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
            if (produit.itemData.equipmentType != EquipmentType.Arrow &&
                produit.itemData.equipmentType != EquipmentType.Shield &&
                produit.itemData.equipmentType != EquipmentType.None)
                VerifItemData(produit.itemData);
        }

        // WEAPONS
        foreach (ItemInInventory produit in Palette.instance.weapons)
        {
            VerifItemData(produit.itemData);
        }

        // EQUIPMENT

        VerifItemData(Equipment.instance.equipmentHeadItem);
        VerifItemData(Equipment.instance.equipmentChestItem);
        VerifItemData(Equipment.instance.equipmentHandsItem);
        VerifItemData(Equipment.instance.equipmentLegsItem);
        VerifItemData(Equipment.instance.equipmentFeetItem);

        if (VerifIfEmpty())
        {
            EndCommerce();
        }
    }

    private void VerifItemData(ItemData item)
    {
        if (item != null)
        {
            GameObject produitItem = Instantiate(produitItemPrefab, parentsProduits.transform);
            SlotAmelioration slot = produitItem.GetComponent<SlotAmelioration>();

            slot.nameText.text = item.itemName; 
            
            slot.icone.sprite = item.visual; 
            
            slot.prixText.text = $"Accepter pour : {prixDeLAmelioration} pièces ?"; 
            
            Button button = slot.acceptButton;
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => Upgrade(item));
                if (PlayerStats.instance.goldAmount < prixDeLAmelioration || item.levelAmelioration == 3)
                {
                    button.interactable = false;
                    button.GetComponent<Image>().color = Color.red;
                    
                }
                else
                {
                    button.interactable = true;
                    button.GetComponent<Image>().color = Color.green; 
                    if (button.gameObject.TryGetComponent<UISelectable>(out var uiSelectable))
                    {
                        navManager.elements.Add(uiSelectable);
                    }
                }
            }

            TextMeshProUGUI statsActuelsText = slot.statsActuels;
            if (statsActuelsText != null)
            {
                if (item.equipmentType == EquipmentType.Weapon)
                {
                    if (item.handWeaponType == HandWeapon.Bow)
                    {
                        statsActuelsText.text = $"Dégâts actuels : {item.damageMax}\nPortée actuelle : { item.rangeMax}";
                    }
                    else
                    {
                        statsActuelsText.text = $"Dégâts actuels : {item.attackPoints}";
                    }
                }
                else
                    statsActuelsText.text = $"Résistance actuelle : {item.armorPoints}"; 
            }

            TextMeshProUGUI statsAmelioresText = slot.statsAmeliores;
            if (statsAmelioresText != null)
            {
                if (item.equipmentType == EquipmentType.Weapon)
                {
                    if (item.handWeaponType == HandWeapon.Bow)
                    {
                        if (item.levelAmelioration == 3)
                            statsAmelioresText.text = $"Dégâts améliorés : {item.damageMax}\nPortée amélioré : {item.rangeMax}";
                        else
                            statsAmelioresText.text = $"Dégâts améliorés : {item.damageMax + upGradeAmount}\nPortée amélioré : {item.rangeMax + upGradeAmount} ";
                    }
                    else
                    {
                        if (item.levelAmelioration == 3)
                            statsAmelioresText.text = $"Dégâts après amélioration : {item.attackPoints}";
                        else
                            statsAmelioresText.text = $"Dégâts après amélioration : {item.attackPoints + upGradeAmount}";
                    }
                }
                else
                {
                    if (item.levelAmelioration == 3)
                        statsAmelioresText.text = $"Résistance après amélioration : {item.armorPoints}"; // Assign the price text
                    else
                        statsAmelioresText.text = $"Résistance après amélioration : {item.armorPoints + upGradeAmountPourcentage}"; // Assign the price text
                }
            }
            TextMeshProUGUI niveauActuelText = slot.niveauActuel;
            if (niveauActuelText != null)
            {
                niveauActuelText.text = $"Niveau actuel : {item.levelAmelioration}/3";
            }
            TextMeshProUGUI niveauAmelioreText = slot.niveauAmeliore;
            if (niveauAmelioreText != null)
            {
                if (item.levelAmelioration == 3)
                    niveauAmelioreText.text = $"Niveau après amélioration : {item.levelAmelioration}/3";
                else
                    niveauAmelioreText.text = $"Niveau après amélioration : {item.levelAmelioration + 1}/3";
            }


            Button resetButton = slot.resetButton;
            if (item.levelAmelioration == 3)
            {
                resetButton.gameObject.SetActive(true);
                resetButton.onClick.RemoveAllListeners();
                resetButton.onClick.AddListener(() =>
                {
                    item.levelAmelioration = 0;
                    if (item.equipmentType == EquipmentType.Weapon)
                    {
                        item.attackPoints -= upGradeAmount * 3;
                    }
                    else
                    {
                        HandleArmorReset(item);
                    }
                    item.prix -= prixDeLAmelioration * 3;
                    UpdateText(item);
                    RefreshProduits();
                });
                if (resetButton.gameObject.TryGetComponent<UISelectable>(out var uiSelectable))
                {
                    navManager.elements.Add(uiSelectable);
                }
            }
            else
                resetButton.gameObject.SetActive(false);
        }
    }
    private void Upgrade(ItemData produit)
    {
        if (produit.equipmentType == EquipmentType.Weapon)
        {
            if(produit.handWeaponType == HandWeapon.Bow)
            {
                produit.rangeMin += upGradeAmount;
                produit.rangeMax += upGradeAmount;
                produit.damageMin += upGradeAmount;
                produit.damageMax += upGradeAmount;
            }
            else
                produit.attackPoints += upGradeAmount;
        }
        else
        {
            if (Equipment.instance.IsEquipped(produit))
            {
                Equipment.instance.DesequipEquipment(produit.equipmentType);
                produit.armorPoints += upGradeAmountPourcentage;
                Equipment.instance.EquipAction(produit);
            }
            else
                produit.armorPoints += upGradeAmountPourcentage;
        }

        produit.prix += prixDeLAmelioration;
        produit.levelAmelioration += 1;
        PlayerStats.instance.goldAmount -= prixDeLAmelioration;
        PlayerStats.instance.UpdateGoldText();
        UpdateText(produit);
        RefreshProduits();
    }

    private void HandleArmorReset(ItemData item)
    {
        if (Equipment.instance.IsEquipped(item))
        {
            Equipment.instance.DesequipEquipment(item.equipmentType);
            item.armorPoints -= upGradeAmountPourcentage * 3;
            Equipment.instance.EquipAction(item);
        }
    }

    private bool VerifIfEmpty()
    {
        if (Inventory.instance.GetContent().Count == 0)
            return Palette.instance.equipmentWeapon1Item == null && Palette.instance.equipmentWeapon2Item == null && 
                Equipment.instance.equipmentHeadItem == null && Equipment.instance.equipmentChestItem == null &&
                Equipment.instance.equipmentHandsItem == null && Equipment.instance.equipmentLegsItem == null &&
                Equipment.instance.equipmentFeetItem == null ;
        else
        {
            bool isEmpty = true;
            for (int i = 0; i < Inventory.instance.GetContent().Count; i++)
            {
                if (Inventory.instance.GetContent()[i].itemData.equipmentType != EquipmentType.None)
                    isEmpty = false;
            }
            return isEmpty;
        }
    }

    private void UpdateText(ItemData produit)
    {
        if (produit.equipmentType == EquipmentType.Weapon)
        {
            if (produit.handWeaponType == HandWeapon.Bow)
            {
                produit.description =
                $"- Dégâts : { produit.damageMin} - { produit.damageMax}\n" +
                $"- Portée : {produit.rangeMin} - {produit.rangeMax}\n";
            }
            else
            {
                produit.description =
                $"- {produit.attackPoints} points de dégâts\n" +
                $"- Type : {produit.damageType}\n";
            }
        }
        else if (produit.equipmentType != EquipmentType.None &&
            produit.equipmentType != EquipmentType.Shield &&
            produit.equipmentType != EquipmentType.Weapon)
        {
            produit.description =
            $"- {produit.armorPoints} points de résistance\n" +
            $"- Type : {produit.armorType}\n";
        }
        produit.description += $"- Niveau : {produit.levelAmelioration}/3";
    }
}
