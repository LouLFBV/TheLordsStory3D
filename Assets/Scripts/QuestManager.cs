using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public List<QuestInstance> activeQuests = new List<QuestInstance>();
    public List<QuestInstance> completedQuests = new List<QuestInstance>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Ajouter une quête
    public void AddQuest(QuestSO questData)
    {
        if (activeQuests.Exists(q => q.data == questData)) return;

        QuestInstance newQuest = new QuestInstance
        {
            data = questData,
            status = QuestStatus.InProgress,
            currentCount = 0
        };

        activeQuests.Add(newQuest);
        Debug.Log($"Nouvelle quête acceptée : {questData.questName}");
    }

    public QuestInstance GetQuestInstance(QuestSO questData)
    {
        return activeQuests.Find(q => q.data == questData) ?? completedQuests.Find(q => q.data == questData);
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
        if(quest.data.questType == QuestType.Interaction)
        { 
            quest = activeQuests.Find(q => q.data == quest.data);
        }
        return quest.data.IsComplete(quest.currentCount ,quest.interactionDone, quest.escortFinished);
    }



    // Marquer comme terminée
    public void CompleteQuest(QuestInstance quest)
    {
        quest.status = QuestStatus.Completed;
        completedQuests.Add(quest);

        QuestInstance toRemove = activeQuests.Find(q => q.data == quest.data);
        if (toRemove != null)
            activeQuests.Remove(toRemove);
    }

    public void ApplyRewards(QuestReward rewards)
    {
        if (rewards == null) return;

        PlayerStats.instance.reputationData.reputationPoints += rewards.reputation;
        if (rewards.gold > 0)
            PlayerStats.instance.AddGold(rewards.gold);

        if (rewards.items != null)
        {
            foreach (var item in rewards.items)
            {
                Inventory.instance.AddItem(item);
            }
        }
    }
}
