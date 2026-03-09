using System.Collections.Generic;
using UnityEngine;

public class NewQuestManager : MonoBehaviour
{
    public static NewQuestManager instance;

    public List<QuestInstance> activeQuests = new List<QuestInstance>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Ajouter une quête
    public void AddQuest(QuestSO questData)
    {
        if (activeQuests.Exists(q => q.data == questData))
            return;

        activeQuests.Add(new QuestInstance
        {
            data = questData,
            status = QuestStatus.InProgress,
            currentCount = 0
        });
        NewQuestLog.instance.CreateQuestButton(activeQuests[activeQuests.Count - 1]);
    }

    public void MarkInteractionDone(QuestSO quest)
    {
        QuestInstance q = activeQuests.Find(x => x.data == quest);
        if (q == null) return;

        q.interactionDone = true;
    }


    public QuestInstance GetQuestInstance(QuestSO questData)
    {
        return activeQuests.Find(q => q.data == questData);
    }

    // Vérifier progression (ex : ramasser un objet, tuer un ennemi…)
    public void UpdateQuestProgress(string target, int amount = 1, ItemData itemDataTarget = null)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.status != QuestStatus.InProgress) continue;

            switch (quest.data.questType)
            {
                case QuestType.Craft:
                case QuestType.Collect:
                    if (quest.data.requiredItem != null && quest.data.requiredItem == itemDataTarget)
                        quest.currentCount += amount;
                    break;

                case QuestType.Hunt:
                    if (quest.data.targetEnemyType.ToString() == target)
                        quest.currentCount += amount;
                    break;
            }
        }
    }

    public bool CanCompleteQuest(QuestInstance quest)
    {
        if (quest == null || quest.status != QuestStatus.InProgress)
            return false;
        if (quest.data.questType == QuestType.Interaction)
        {
            quest = activeQuests.Find(q => q.data == quest.data);
        }
        return quest.data.IsComplete(quest.currentCount, quest.interactionDone, quest.escortFinished);
    }



    // Marquer comme terminée
    public void CompleteQuest(QuestInstance quest)
    {
        quest.status = QuestStatus.Completed;
        QuestInstance toRemove = activeQuests.Find(q => q.data == quest.data);
        if (toRemove != null)
            activeQuests.Remove(toRemove);
    }

    public void ApplyRewards(QuestInstance questInstane)
    {
        if (questInstane.data.rewards == null) return;

        PlayerStats.instance.reputationData.reputationPoints += questInstane.data.rewards.reputation;
        if (questInstane.data.rewards.gold > 0)
            PlayerStats.instance.AddGold(questInstane.data.rewards.gold);

        if (questInstane.data.rewards.items != null)
        {
            foreach (var item in questInstane.data.rewards.items)
            {
                Inventory.instance.AddItem(item);
            }
        }
        questInstane.rewardsGiven = true;
    }

    #region Save/Load
    public QuestSaveData GetSaveData()
    {
        QuestSaveData data = new QuestSaveData();

        foreach (var quest in activeQuests)
        {
            data.activeQuests.Add(ToSaveData(quest));
        }

        return data;
    }
    private QuestInstanceSaveData ToSaveData(QuestInstance quest)
    {
        return new QuestInstanceSaveData
        {
            questID = quest.data.questID,
            status = quest.status,
            currentCount = quest.currentCount,
            interactionDone = quest.interactionDone,
            escortFinished = quest.escortFinished,
            rewardsGiven = quest.rewardsGiven
        };
    }


    public void LoadSaveData(QuestSaveData data)
    {
        if (data == null) return;

        activeQuests.Clear();

        HashSet<string> completedIDs = new();


        foreach (var questData in data.activeQuests)
        {
            if (completedIDs.Contains(questData.questID)) continue;

            QuestInstance quest = FromSaveData(questData);
            if (quest != null)
                activeQuests.Add(quest);
        }
    }



    private QuestInstance FromSaveData(QuestInstanceSaveData data)
    {
        QuestSO questSO = QuestDatabase.Instance.GetQuestByID(data.questID);
        if (questSO == null)
        {
            Debug.LogWarning($"Quest not found: {data.questID}");
            return null;
        }

        return new QuestInstance
        {
            data = questSO,
            status = data.status,
            currentCount = data.currentCount,
            interactionDone = data.interactionDone,
            escortFinished = data.escortFinished,
            rewardsGiven = data.rewardsGiven
        };
    }
    #endregion
}

[System.Serializable]
public class QuestSaveData
{
    public List<QuestInstanceSaveData> activeQuests = new();
    public List<QuestInstanceSaveData> completedQuests = new();
}
