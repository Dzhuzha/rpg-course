using System;
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
        [SerializeField] private TMP_Text _rewardText;

        public void Setup(QuestStatus questStatus)
        {
            QuestStatus status = questStatus;
            _title.text = status.Quest.Name;

            for (int i = 0; i < _container.childCount; i++)
            {
                Destroy(_container.GetChild(i));
            }

            foreach (Quest.Objective objective in status.Quest.GetObjectives())
            {
                ObjectiveWidget objectiveWidget = Instantiate(_objectivePrefab, _container);
                objectiveWidget.SetObjectiveText(objective.Description);
                objectiveWidget.SetCompleteMark(status.IsObjectiveComplete(objective.Reference));
            }

            _rewardText.text = GetRewardText(questStatus); 
        }

        private string GetRewardText(QuestStatus quest)
        {
            string rewardText = String.Empty;

            foreach (Quest.Reward reward in quest.Quest.GetReward())
            {
                if (rewardText != String.Empty)
                {
                    rewardText += ", ";
                }

                rewardText += reward.Item.DisplayName;

                if (reward.Number > 1)
                {
                    rewardText += $"({reward.Number})";
                }
            }
            
            return rewardText == String.Empty? "No reward" : rewardText;
        }
    }
}