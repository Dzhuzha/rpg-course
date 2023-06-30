using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 52)]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] _characterClasses;
        
        public float GetHealth(CharacterClass characterClass, int level)
        {
            foreach (var progressionCharacterClass in _characterClasses)
            {
                if (progressionCharacterClass.ClassName != characterClass) continue;

                return progressionCharacterClass.GetHealth(level);
            }

            return 0;
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] private CharacterClass _className;
            [SerializeField] private float[] _healthPerLevel;
            [SerializeField] private float[] _damagePerLevel;
            [SerializeField] private int _characterLevel;
            
            public CharacterClass ClassName => _className;

            public float GetHealth(int level)
            {
                return _healthPerLevel[level - 1];
            }
        }
    }
}