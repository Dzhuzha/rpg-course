using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99), SerializeField] private int _startingLevel = 1;
        [SerializeField] private CharacterClass _characterClass;
        [SerializeField] private Progression _progression;
        [SerializeField] private Experience _experience;

        private int _currentLevel;

        public event Action<int> LevelChanged;

        private void Start()
        {
            Subscribe();
            _currentLevel = GetLevel();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_experience == null) return;
            _experience.ExperienceChanged += UpdateLevel;
        }

        private void Unsubscribe()
        {
            if (_experience == null) return;
            _experience.ExperienceChanged -= UpdateLevel;
        }

        private void UpdateLevel(float experience)
        {
            int newLevel = GetLevel();
            
            if (newLevel > _currentLevel)
            {
                _currentLevel = newLevel;
            }
            
            LevelChanged?.Invoke(newLevel);
        }

        public float GetStat(Stat stat)
        {
            return _progression.GetStat(stat, _characterClass, GetLevel());
        }

        private int GetLevel() 
        {
            if (_experience == null)
            {
                return _startingLevel;
            }
            
            float penultimateLevel = _progression.GetLevels(Stat.ExperienceToLevelUp, _characterClass);
            
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = _progression.GetStat(Stat.ExperienceToLevelUp, _characterClass, level);
                
                if (XPToLevelUp > _experience.ExperiencePoints)
                {
                    return level;
                }
            }
            
            return (int)penultimateLevel + 1;
        }
    }
}