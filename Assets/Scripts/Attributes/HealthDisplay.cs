using TMPro;
using UnityEngine;

namespace RPG.Atributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthValue;
        [SerializeField] private Health _health;
        
        private void Start()
        {
            Subscribe();
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
            _health.HealthChanged -= UpdateHealthValue;
        }

        private void UpdateHealthValue(float newHealthValue)
        {
            _healthValue.text = string.Concat(newHealthValue.ToString("N0"), "%");
        }
    }
}