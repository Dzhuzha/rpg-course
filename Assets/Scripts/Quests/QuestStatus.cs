using System.Collections.Generic;

namespace RPG.Quests
{
    public class QuestStatus
    {
        private List<string> _completedObjectives = new List<string>();

        public Quest Quest { get; private set; } 
        public int CompletedObjectivesCount => _completedObjectives.Count;

        public QuestStatus(Quest questToSetup)
        {
            Quest = questToSetup;
        }
        
        public bool IsObjectiveComplete(string objective)
        {
            return _completedObjectives.Contains(objective);
        }
    }
}