using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 52)]
    public class Progression : ScriptableObject
    {
        [NonReorderable, SerializeField] private ProgressionCharacterClass[] _characterClasses;

        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookUp();
            float[] levels = _lookupTable[characterClass][stat];

            if (levels.Length < level) return 0;
            return levels[level-1];
        }

        private void BuildLookUp()
        {
            if (_lookupTable != null) return;

            _lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass characterClass in _characterClasses)
            {
                Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat stat in characterClass.Stats)
                {
                    statLookupTable[stat.Stat] = stat.Levels;
                    _lookupTable[characterClass.ClassName] = statLookupTable;
                }
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] private CharacterClass _className;

            [NonReorderable, SerializeField] private float[] _damagePerLevel;
            [SerializeField] private int _characterLevel;


            [NonReorderable, SerializeField] private ProgressionStat[] _stats;
            public CharacterClass ClassName => _className;
            public ProgressionStat[] Stats => _stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            [SerializeField] private Stat _stat;
            [NonReorderable, SerializeField] private float[] _levels;

            public Stat Stat => _stat;
            public float[] Levels => _levels;
        }
    }
}