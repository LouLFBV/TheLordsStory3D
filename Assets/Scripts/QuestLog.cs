using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLog : MonoBehaviour
{
    public static QuestLog instance;

    [Header("UI Texte")]
    public TextMeshProUGUI QuestActiveText;
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questObligatoireText;

    [Header("UI Panels & Prefabs")]
    [SerializeField] private GameObject panelDescriptionQuest;
    [SerializeField] private Transform QuestsList;
    [SerializeField] private GameObject buttonQuestPrefab;
    [SerializeField] private GameObject gameObjectPourAfficher;
    [SerializeField] private TextMeshProUGUI compteurEnemiesText;
    public Toggle questToggle;

    [Header("Boutons Onglets")]
    [SerializeField] private Button buttonActivesQuests;
    [SerializeField] private Button buttonCompletedQuests;

    [SerializeField] private UINavigationManager uiNavigationManager;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void ShowQuest(QuestInstance quest, bool isActive)
    {
        if (quest == null) return;

        panelDescriptionQuest.SetActive(true);
        questNameText.text = quest.data.questName;
        questDescriptionText.text = quest.data.description;
        questObligatoireText.text = quest.data.isMainQuest ? "Quête Obligatoire" : "Quête Secondaire";

        if (quest.data.questType == QuestType.Hunt && isActive)
        {
            compteurEnemiesText.gameObject.SetActive(true);
            QuestInstance questInstance = QuestManager.instance.GetQuestInstance(quest.data);
            compteurEnemiesText.text = $"Ennemis tués : {questInstance.currentCount} / {quest.data.requiredKillCount}";
        }
        else
        {
            compteurEnemiesText.gameObject.SetActive(false);
        }

        // Active = toggle disponible, Completed = pas de toggle
        gameObjectPourAfficher.SetActive(isActive);

        if (isActive)
        {
            questToggle.onValueChanged.RemoveAllListeners();
            questToggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                    ActiveDesactiveQuestText(questNameText.text);
                else
                    QuestActiveText.gameObject.SetActive(false);
            });
        }
    }

    private void CreateQuestButton(QuestInstance quest, bool isActive)
    {
        GameObject button = Instantiate(buttonQuestPrefab, QuestsList);
        button.GetComponentInChildren<TextMeshProUGUI>().text = quest.data.questName;
        uiNavigationManager.elements.Add(button.GetComponent<UISelectable>());
        button.GetComponent<Button>().onClick.RemoveAllListeners();
        button.GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowQuest(quest, isActive);
        });
    }
    private void RefreshQuestList(System.Collections.Generic.List<QuestInstance> quests, bool isActive)
    {
        foreach (Transform child in QuestsList)
            Destroy(child.gameObject);

        foreach (var quest in quests)
            CreateQuestButton(quest, isActive);
    }

    // --- Appelées depuis UI ---
    public void ButtonActiveQuests()
    {
        RefreshQuestList(QuestManager.instance.activeQuests, true);
        panelDescriptionQuest.SetActive(false);
    }

    public void ButtonCompletedQuests()
    {
        RefreshQuestList(QuestManager.instance.completedQuests, false);
        panelDescriptionQuest.SetActive(false);
    }

    public void ActiveDesactiveQuestText(string questText)
    {
        QuestActiveText.text = questText;
        QuestActiveText.gameObject.SetActive(true);
    }

    public void OnAffichage()
    {
        panelDescriptionQuest.SetActive(false);
        foreach (Transform child in QuestsList)
            Destroy(child.gameObject);
    }
}
