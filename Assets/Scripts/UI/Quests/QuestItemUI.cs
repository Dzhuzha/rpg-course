using System;
using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _progress;

        public QuestStatus QuestStatus { get; private set; }

        public void Setup(QuestStatus questStatus)
        {
            QuestStatus = questStatus;
            _title.text = QuestStatus.Quest.Name;
            _progress.text = String.Concat(QuestStatus.CompletedObjectivesCount, "/", QuestStatus.Quest.ObjectivesCount);
        }
    }
}