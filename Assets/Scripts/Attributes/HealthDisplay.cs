using TMPro;
using UnityEngine;

namespace RPG.Atributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthValue;
        [SerializeField] private Health _health;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _health.HealthChanged += UpdateHealthValue;
        }
        
        private void Unsubscribe()
        {
            _health.HealthChanged -= UpdateHealthValue;
        }

        private void UpdateHealthValue(float newHealthValue, float fullHealthValue)
        {
            _healthValue.text = string.Concat(newHealthValue.ToString("N0"), "/", fullHealthValue.ToString("N0"));
        }
    }
}