using UnityEngine;

public enum QuestType {Collect, Hunt, Craft, Interaction}

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class QuestSO : ScriptableObject
{
    [Header("Identification")]
    public string questID;
    public string questName;
    [TextArea] public string description;


    [Header("Dialogues")]
    public DialogueResponse[] sentencesBeforeQuest;     // proposition
    public DialogueResponse[] sentencesQuestInProgress; // en cours
    public DialogueResponse[] sentencesQuestCompleted;  // réussite
    public DialogueResponse[] sentencesQuestAccepted;
    public DialogueResponse[] sentencesQuestRefused;

    [Header("Dialogues pour pnj à aller voir")]
    public DialogueResponse[] sentencesInteraction;


    [Header("Type de quête")]
    public QuestType questType;
    public bool isMainQuest; // True = quête de l’histoire

    [Header("Conditions")]
    [Header("Collect/Craft")]
    public ItemData requiredItem;  // ex: item à ramener (si c’est une Collect/Craft)
    public int requiredItemCount;
    [Header("Hunt")]
    public EnemyType targetEnemyType;  // ex: "Wolf" si c’est une Hunt
    public int requiredKillCount;
    [Header("Interaction")]
    public string namePNJ;       // nom/ID du PNJ à aller voir (si Interaction/Escort)

    [Header("Récompenses")]
    public QuestReward rewards;

    public bool IsComplete(int currentCount, bool interactionDone = false, bool escortFinished = false)
    {
        switch (questType)
        {
            case QuestType.Interaction:
                return interactionDone;

            case QuestType.Collect:
            case QuestType.Craft:
                return currentCount >= requiredItemCount;

            case QuestType.Hunt:
                return currentCount >= requiredKillCount;

            default:
                return false;
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(questID))
            questID = System.Guid.NewGuid().ToString();
    }
#endif
}

[System.Serializable]
public class QuestReward
{
    public int reputation;
    public int gold;
    public ItemData[] items;
}