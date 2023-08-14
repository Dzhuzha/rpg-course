using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float _experiencePoints = 0f;

        public float ExperiencePoints => _experiencePoints;

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

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_experiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            _experiencePoints = state.ToObject<float>();
        }
    }
}