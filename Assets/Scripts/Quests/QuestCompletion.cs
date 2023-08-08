using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest _quest;
        [SerializeField] private string _objective;

        private QuestList _playerQuests;
        
        private void Awake()
        {
            _playerQuests = FindObjectOfType<QuestList>();
        }

        public void CompleteObjective()
        {
            _playerQuests.CompleteObjective(_quest, _objective);
        }
    }
}