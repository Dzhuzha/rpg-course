using System;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using RPG.Core;

namespace RPG.Atributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _health = 100f;

        private ActionScheduler _actionScheduler;
        private BaseStats _baseStats;

        public bool IsDead => _health <= 0;
        
        public event Action<float> HealthChanged;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _baseStats = GetComponent<BaseStats>();
            _health = _baseStats.GetHealth();
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            CheckDeathState();
            UpdateHealthPercentage();
        }

        public void UpdateHealthPercentage()
        {
            if (_baseStats == null) return;

            float healthPercentage = _health / _baseStats.GetHealth() * 100;
            HealthChanged?.Invoke(healthPercentage);
        }

        private void CheckDeathState()
        {
            if (!IsDead) return;

            Die();
            _health = 0;
        }

        private void Die()
        {
            _actionScheduler.CancelCurrentAction();
            _animator.SetTrigger("Death");
        }

        private void ForceDead()
        {
            _actionScheduler.CancelCurrentAction();
            _animator.SetTrigger("Dead"); 
        }

        public object CaptureState()
        {
            return _health as object;
        }

        public void RestoreState(object state)
        {
            _health = (float) state;
            UpdateHealthPercentage();
            if (!IsDead) return;
            ForceDead();
        }
    }
}