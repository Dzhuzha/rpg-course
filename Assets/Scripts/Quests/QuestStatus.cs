using System;
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
        
        public QuestStatus(object objectState)
        {
            QuestStatusRecord questRecord = objectState as QuestStatusRecord;
           
            if (questRecord != null)
            {
                Quest = Quest.GetByName(questRecord.QuestName);
                _completedObjectives = questRecord.CompletedObjectives;
            }
        }
        
        public bool IsObjectiveComplete(string objective)
        {
            return _completedObjectives.Contains(objective);
        }

        public void CompletedObjective(string objective)
        {
            if (Quest.IsContainObjective(objective) == false || _completedObjectives.Contains(objective)) return;

            _completedObjectives.Add(objective);
        }

        [Serializable]
        private class QuestStatusRecord
        {
            public string QuestName;
            public List<string> CompletedObjectives;
        }

        public object CaptureState()
        {
           QuestStatusRecord state = new QuestStatusRecord();
           state.QuestName = Quest.Name;
           state.CompletedObjectives = _completedObjectives;

           return state;
        }
    }
}