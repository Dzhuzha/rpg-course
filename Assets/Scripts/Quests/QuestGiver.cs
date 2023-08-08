using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private Quest _questToGive;

        public void GiveQuest()
        {
            QuestList questList = FindObjectOfType<QuestList>();
            
            questList.AddQuest(_questToGive);
        }
    }
}