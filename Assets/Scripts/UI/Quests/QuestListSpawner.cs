using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListSpawner : MonoBehaviour
    {
        [SerializeField] private QuestItemUI _questItem;

        private void Start()
        {
            QuestList playerQuests = FindObjectOfType<QuestList>();
            
            if (playerQuests.QuestCount < 1) return;

            foreach (QuestStatus status in playerQuests.GetQuests())
            {
                QuestItemUI questUI = Instantiate(_questItem, transform);
                questUI.Setup(status);   
            }
        }
    }
}