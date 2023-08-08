using System;
using System.Collections.Generic;
using RPG.Inventory;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quests", menuName = "Quests/Create New Quest", order = 54)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField, NonReorderable] private List<Objective> _objectives = new List<Objective>();
        [SerializeField, NonReorderable] private List<Reward> _rewards = new List<Reward>();

        public string Name => _name;
        public int ObjectivesCount => _objectives.Count;

        [Serializable]
        public class Reward
        {
            [Min(1)] public int Number;
            public InventoryItem Item;
        }
        
        [Serializable]
        public class Objective
        {
            public string Reference;
            public string Description;
        }
        
        public IEnumerable<Objective> GetObjectives()
        {
            return _objectives;
        }

        public IEnumerable<Reward> GetReward()
        {
            return _rewards;
        }

        public bool IsContainObjective(string objectiveReference)
        {
            foreach (Objective objectiveItem in _objectives)
            {
                if (objectiveItem.Reference == objectiveReference)
                {
                    return true;
                }
            }
            
            return false;
        }

        public static Quest GetByName(string questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.Name == questName)
                {
                    return quest;
                }
            }

            return null;
        }
    }
}