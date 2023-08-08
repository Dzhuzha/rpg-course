using System;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListSpawner : MonoBehaviour
    {
        [SerializeField] private QuestItemUI _questItem;

        private QuestList _playerQuests;
        
        private void Start()
        {
            _playerQuests = FindObjectOfType<QuestList>();
            Subscribe();
            UpdateQuestUI();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerQuests.QuestListUpdated += UpdateQuestUI;
        }

        private void Unsubscribe()
        {
            _playerQuests.QuestListUpdated -= UpdateQuestUI;
        }

        private void UpdateQuestUI()
        {
            if (_playerQuests.QuestCount < 1) return;

            foreach (QuestStatus status in _playerQuests.GetQuests())
            {
                QuestItemUI questUI = Instantiate(_questItem, transform);
                questUI.Setup(status);   
            }
        }
    }
}