using System;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable
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

        object ISaveable.CaptureState()
        {
            List<object> state = new List<object>();

            foreach (QuestStatus status in _statuses)
            {
                state.Add(status.CaptureState());
            }

            return state;
        }

        void ISaveable.RestoreState(object state)
        {
            List<object> stateList = state as List<object>;

            if (stateList == null) return;

            _statuses.Clear();
            foreach (object objectState in stateList)
            {
                _statuses.Add(new QuestStatus(objectState));
            }
        }
    }
}