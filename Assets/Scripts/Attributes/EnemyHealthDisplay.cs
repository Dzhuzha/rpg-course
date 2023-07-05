using TMPro;
using UnityEngine;

namespace RPG.Atributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthValue;
        private Health _health;

        public void InitTarget(Health health)
        {
            Unsubscribe();
            
            _health = health;
            Subscribe();
            _health.UpdateHealthPercentage();
        }

        public void ResetTarget()
        {
            Unsubscribe();
            _health = null;
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _health.HealthChanged += UpdateHealthValue;
        }

        private void Unsubscribe()
        {
            if (_health == null) return;
            
            _health.HealthChanged -= UpdateHealthValue;
        }
        
        private void UpdateHealthValue(float newHealthValue, float fullHealthValue)
        {
            _healthValue.text = string.Concat(newHealthValue.ToString("N0"), "/", fullHealthValue.ToString("N0"));
        }

        private void UpdateHealthValue(float newHealthValue)
        {
            _healthValue.text = string.Concat(newHealthValue.ToString("N0"), "%");
        }
    }
}