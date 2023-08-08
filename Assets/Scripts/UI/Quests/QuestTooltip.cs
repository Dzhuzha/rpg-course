using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Transform _container;
        [SerializeField] private ObjectiveWidget _objectivePrefab;

        public void Setup(QuestStatus questStatus)
        {
            QuestStatus status = questStatus;
            _title.text = status.Quest.Name;

            for (int i = 0; i < _container.childCount; i++)
            {
                Destroy(_container.GetChild(i));
            }

            foreach (string objective in status.Quest.GetObjectives())
            {
                ObjectiveWidget objectiveWidget = Instantiate(_objectivePrefab, _container);
                objectiveWidget.SetObjectiveText(objective);
                objectiveWidget.SetCompleteMark(status.IsObjectiveComplete(objective));
            }
        }
    }
}