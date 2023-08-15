using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Inventory;
using RPG.Saving;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, IPredicateEvaluator, IJsonSaveable
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

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "HasQuest":
                    return HasQuest(Quest.GetByName(parameters[0]));
                case "CompletedQuest":
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsQuestComplete();
            }

            return null;
        }

        public JToken CaptureAsJToken()
        {
           JArray state = new JArray();
           IList<JToken> stateList = state;

           foreach (QuestStatus status in _statuses)
           {
               stateList.Add(status.CaptureAsJToken());
           }

           return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JArray stateArray)
            {
                _statuses.Clear();
                IList<JToken> stateList = stateArray;

                foreach (JToken token in stateList)
                {
                    _statuses.Add(new QuestStatus(token));
                }
            }
        }
    }
}