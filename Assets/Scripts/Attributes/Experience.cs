using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float _experiencePoints = 0f;
        
        public event Action<float> ExperienceChanged;
        
        private void Start()
        {
            ExperienceChanged?.Invoke(_experiencePoints);
        }
        
        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
            ExperienceChanged?.Invoke(_experiencePoints);
        }

        public object CaptureState()
        {
            return _experiencePoints as object;
        }

        public void RestoreState(object state)
        {
            _experiencePoints = (float)state;
        }
    } 
}