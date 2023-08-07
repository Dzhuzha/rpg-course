using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quests", menuName = "Quests/Create New Quest", order = 54)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string[] _objectives;

        public string Name => _name;
        public string[] Objectives => _objectives;
    }
}