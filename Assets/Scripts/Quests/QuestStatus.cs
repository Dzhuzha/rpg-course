using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [Serializable]
    public class QuestStatus
    {
        [SerializeField] private Quest _quest;
        [SerializeField, NonReorderable] private List<string> _completedObjectives;

        public Quest Quest => _quest;
        public int CompletedObjectivesCount => _completedObjectives.Count;

        public bool IsObjectiveComplete(string objective)
        {
            return _completedObjectives.Contains(objective);
        }
    }
}