using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 52)]
    public class Progression : ScriptableObject
    {
        [NonReorderable, SerializeField] private ProgressionCharacterClass[] _characterClasses;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            foreach (var progressionCharacterClass in _characterClasses)
            {
                if (progressionCharacterClass.ClassName != characterClass) continue;

                foreach (var progressionStat in progressionCharacterClass.Stats)
                { 
                    if (progressionStat.Stat != stat) continue;
                    if (progressionStat.Levels.Length < level) continue;
                    
                    return progressionStat.Levels[level - 1];
                }

                // return progressionCharacterClass.GetHealth(level);
            }

            return 0;
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] private CharacterClass _className;

            //  [SerializeField] private float[] _healthPerLevel;
            [NonReorderable, SerializeField] private float[] _damagePerLevel;
            [SerializeField] private int _characterLevel;


            [NonReorderable, SerializeField] private ProgressionStat[] _stats;
            public CharacterClass ClassName => _className;
            public ProgressionStat[] Stats => _stats;

            public float GetHealth(int level)
            {
                // return _healthPerLevel[level - 1];
                return _stats[0].Levels[level - 1];
            }
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