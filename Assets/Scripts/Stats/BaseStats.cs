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
        [SerializeField] private GameObject _levelUpEffect;
        [SerializeField] private bool _shouldUseModifiers = false;
        
        public int CurrentLevel { get; private set; }

        public event Action<int> LevelChanged;

        private void Start()
        {
            Subscribe();
            CurrentLevel = CalculateLevel();
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
            int newLevel = CalculateLevel();
            
            if (newLevel > CurrentLevel)
            {
                CurrentLevel = newLevel;
                CallLevelUpEffect();
                LevelChanged?.Invoke(newLevel);
            }
        }

        private void CallLevelUpEffect()
        {
            if (_levelUpEffect == null) return;
            Instantiate(_levelUpEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * GetPercentageModifier(stat);
        }

        private float GetBaseStat(Stat stat)
        {
           return _progression.GetStat(stat, _characterClass, CalculateLevel());
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (_shouldUseModifiers == false) return 1;
            
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            float percentModifier = 0f;
            
            foreach (IModifierProvider provider in providers)
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    percentModifier += modifier;
                }
            }
            
            return 1 + percentModifier / 100;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (_shouldUseModifiers == false) return 0;
            
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            float total = 0f;
            
            foreach (IModifierProvider provider in providers)
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            
            return total;
        }

        private int CalculateLevel() 
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