using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _progress;

        public void Setup(Quest quest)
        {
            _title.text = quest.Name;
            _progress.text = quest.Objectives.Length.ToString();
        }
    }
}