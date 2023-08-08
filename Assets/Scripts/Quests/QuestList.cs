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

            if (_statuses.Find(q => q.Quest.Name == newQuestStatus.Quest.Name) != null) return;

            _statuses.Add(newQuestStatus);
            QuestListUpdated?.Invoke();
        }
        
        public IEnumerable<QuestStatus> GetQuests()
        {
            return _statuses;
        }
    }
}