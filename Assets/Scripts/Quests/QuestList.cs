using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour
    {
        [SerializeField, NonReorderable] private QuestStatus[] _statuses;

        public int QuestCount => _statuses.Length;
        
        public IEnumerable<QuestStatus> GetQuests()
        {
            return _statuses;
        }
    }
}