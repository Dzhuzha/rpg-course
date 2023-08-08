using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour
    {
        public event Action QuestListUpdated;

        private List<QuestStatus> _statuses = new List<QuestStatus>();

        public int QuestCount => _statuses.Count;

        public void AddQuest(Quest questToAdd)
        {
            QuestStatus newQuestStatus = new QuestStatus(questToAdd);

            if (GetQuestStatus(questToAdd) != null) return;

            _statuses.Add(newQuestStatus);
            QuestListUpdated?.Invoke();
        }

        public IEnumerable<QuestStatus> GetQuests()
        {
            return _statuses;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus questToComplete = GetQuestStatus(quest);

            if (questToComplete == null) return;

            questToComplete.CompletedObjective(objective);
            QuestListUpdated?.Invoke();
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus questStatus in _statuses)
            {
                if (questStatus.Quest == quest)
                {
                    return questStatus;
                }
            }

            return null;
        }
    }
}