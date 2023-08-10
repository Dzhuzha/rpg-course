using System;
using System.Collections.Generic;
using RPG.Core;
using RPG.Inventory;
using RPG.Saving;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        public event Action QuestListUpdated;

        private List<QuestStatus> _statuses = new List<QuestStatus>();

        public int QuestCount => _statuses.Count;

        public void AddQuest(Quest questToAdd)
        {
            if (HasQuest(questToAdd)) return;
            QuestStatus newQuestStatus = new QuestStatus(questToAdd);
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

            if (questToComplete.IsQuestComplete())
            {
                GiveReward(quest);
            }

            QuestListUpdated?.Invoke();
        }

        private void GiveReward(Quest quest)
        {
            foreach (Quest.Reward reward in quest.GetReward())
            {
                bool itemAdded = GetComponent<RPG.Inventory.Inventory>().AddToFirstEmptySlot(reward.Item, reward.Number);

                if (!itemAdded)
                {
                    GetComponent<ItemDropper>().DropItem(reward.Item, reward.Number);
                }
            }
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

        private bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
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

        public bool? Evaluate(PredicateType predicate, string[] parameters)
        {
            switch (predicate)
            {
                case PredicateType.HasQuest:
                    return HasQuest(Quest.GetByName(parameters[0]));
                case PredicateType.CompletedQuest:
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsQuestComplete();
            }

            return null;
        }
    }
}