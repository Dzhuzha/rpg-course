using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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

        public bool IsQuestComplete()
        {
            foreach (var objective in Quest.GetObjectives())
            {
                if (!_completedObjectives.Contains(objective.Reference))
                {
                    return false;
                }
            }

            return true;
        }

        public QuestStatus(JToken objectState)
        {
            if (objectState is JObject state)
            {
                IDictionary<string, JToken> stateDictionary = state;
                Quest = Quest.GetByName(stateDictionary["questName"].ToObject<string>());
                _completedObjectives.Clear();

                if (stateDictionary["completedObjectives"] is JArray completedState)
                {
                    IList<JToken> completedStateArray = completedState;

                    foreach (JToken objective in completedStateArray)
                    {
                        _completedObjectives.Add(objective.ToObject<string>());
                    }
                }
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

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDictionary = state;
            stateDictionary["questName"] = Quest.Name;
            JArray completedState = new JArray();
            IList<JToken> completedStateArray = completedState;

            foreach (string objective in _completedObjectives)
            {
                completedStateArray.Add(JToken.FromObject(objective));
            }

            return state;
        }
    }
}