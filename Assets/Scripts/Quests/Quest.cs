using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quests", menuName = "Quests/Create New Quest", order = 54)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private List<string> _objectives = new List<string>();

        public string Name => _name;
        public int ObjectivesCount => _objectives.Count;

        public IEnumerable<string> GetObjectives()
        {
            return _objectives;
        }

        public bool IsContainObjective(string objective)
        {
            return _objectives.Contains(objective);
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