using System.Collections.Generic;
using UnityEngine;

public class QuestDatabase : MonoBehaviour
{
    public static QuestDatabase Instance;

    [SerializeField] private QuestSO[] allQuests;

    private Dictionary<string, QuestSO> questByID;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        questByID = new Dictionary<string, QuestSO>();
        foreach (var quest in allQuests)
        {
            if (!questByID.ContainsKey(quest.questID))
                questByID.Add(quest.questID, quest);
        }
    }

    public QuestSO GetQuestByID(string id)
    {
        questByID.TryGetValue(id, out var quest);
        return quest;
    }
}
