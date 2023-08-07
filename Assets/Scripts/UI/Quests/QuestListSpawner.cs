using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListSpawner : MonoBehaviour
    {
        [SerializeField] private Quest[] _tempQuest;
        [SerializeField] private QuestItemUI _questItem;

        private void Start()
        {
            if (_tempQuest.Length < 1) return;

            for (int i = 0; i < _tempQuest.Length; i++)
            {
               QuestItemUI questUI = Instantiate(_questItem, transform);
               questUI.Setup(_tempQuest[i]);
            }
        }
    }
}