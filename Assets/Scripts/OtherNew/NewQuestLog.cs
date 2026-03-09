using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewQuestLog : MonoBehaviour
{
    public static NewQuestLog instance;

    [Header("UI Texte")]
    public TextMeshProUGUI QuestActiveText;
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;

    [Header("UI Panels & Prefabs")]
    [SerializeField] private GameObject panelDescriptionQuest;
    [SerializeField] private Transform questsFirstList;
    [SerializeField] private Transform questsSecondList;
    [SerializeField] private GameObject buttonQuestPrefab;
    [SerializeField] private GameObject objectifQuestPrefab;
    [SerializeField] private GameObject rewardQuestPrefab;
    //[SerializeField] private GameObject gameObjectPourAfficher;
    //[SerializeField] private TextMeshProUGUI compteurEnemiesText;
    //public Toggle questToggle;

    [SerializeField] private UINavigationManager uiNavigationManager;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void ShowQuest(QuestInstance quest/*, bool isActive*/)
    {
        if (quest == null) return;

        panelDescriptionQuest.SetActive(true);
        questNameText.text = quest.data.questName;
        questDescriptionText.text = quest.data.description;
        foreach (var objetif in quest.data.objectifs)
        {
            GameObject obj = Instantiate(objectifQuestPrefab, panelDescriptionQuest.transform);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = $"- {objetif}";
        }
        foreach (var reward in quest.data.rewardsText)
        {
            GameObject obj = Instantiate(rewardQuestPrefab, panelDescriptionQuest.transform);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = $"- {reward}";
        }

        //if (quest.data.questType == QuestType.Hunt && isActive)
        //{
        //    compteurEnemiesText.gameObject.SetActive(true);
        //    QuestInstance questInstance = QuestManager.instance.GetQuestInstance(quest.data);
        //    compteurEnemiesText.text = $"Ennemis tués : {questInstance.currentCount} / {quest.data.requiredKillCount}";
        //}
        //else
        //{
        //    compteurEnemiesText.gameObject.SetActive(false);
        //}

        // Active = toggle disponible, Completed = pas de toggle
        //gameObjectPourAfficher.SetActive(isActive);

        //if (isActive)
        //{
        //    questToggle.onValueChanged.RemoveAllListeners();
        //    questToggle.onValueChanged.AddListener((isOn) =>
        //    {
        //        if (isOn)
        //            ActiveDesactiveQuestText(questNameText.text);
        //        else
        //            QuestActiveText.gameObject.SetActive(false);
        //    });
        //}
    }

    public void CreateQuestButton(QuestInstance quest/*, bool isActive*/)
    {
        GameObject button = null;
        if (quest.data.isMainQuest)
             button = Instantiate(buttonQuestPrefab, questsFirstList);
        else
             button = Instantiate(buttonQuestPrefab, questsSecondList);

        button.GetComponentInChildren<TextMeshProUGUI>().text = quest.data.questName;
        uiNavigationManager.elements.Add(button.GetComponent<UISelectable>());
        button.GetComponent<Button>().onClick.RemoveAllListeners();
        button.GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowQuest(quest/*, isActive*/);
        });
    }

    public void ActiveDesactiveQuestText(string questText)
    {
        QuestActiveText.text = questText;
        QuestActiveText.gameObject.SetActive(true);
    }

    //public void OnAffichage()
    //{
    //    panelDescriptionQuest.SetActive(false);
    //    foreach (Transform child in QuestsList)
    //        Destroy(child.gameObject);
    //}

    #region Save/Load
    public QuestLogSaveData GetSaveData()
    {
        return new QuestLogSaveData
        {
            activeQuestName = QuestActiveText.gameObject.activeSelf
                ? QuestActiveText.text
                : string.Empty,
            //isQuestToggleOn = questToggle != null && questToggle.isOn
        };
    }

    public void LoadSaveData(QuestLogSaveData data)
    {
        if (data == null || string.IsNullOrEmpty(data.activeQuestName))
        {
            QuestActiveText.gameObject.SetActive(false);
            return;
        }

        //  Texte
        QuestActiveText.text = data.activeQuestName;
        QuestActiveText.gameObject.SetActive(true);

        //  Toggle (sans déclencher l’event)
        //if (questToggle != null)
        //{
        //    questToggle.onValueChanged.RemoveAllListeners();
        //    questToggle.isOn = data.isQuestToggleOn;

        //    questToggle.onValueChanged.AddListener(isOn =>
        //    {
        //        if (isOn)
        //            ActiveDesactiveQuestText(QuestActiveText.text);
        //        else
        //            QuestActiveText.gameObject.SetActive(false);
        //    });
        //}
    }
    #endregion
}

[System.Serializable]
public class QuestLogSaveData
{
    public string activeQuestName;
    public bool isQuestToggleOn;
}

