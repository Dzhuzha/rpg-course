using RPG.Quests;
using RPG.UI.Utils.Tooltips;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        
        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus questStatus = GetComponent<QuestItemUI>().QuestStatus;
            tooltip.GetComponent<QuestTooltip>().Setup(questStatus);
        }

        public override bool CanCreateTooltip()
        {
            return true;
        }
    }
}