using System;
using RPG.Stats;
using RPG.Saving;
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
            _health = _baseStats.GetStat(Stat.Health);
            Subscribe();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            _baseStats.LevelChanged += UpdateHealth;
        }
        
        private void UnSubscribe()
        {
            _baseStats.LevelChanged -= UpdateHealth;
        }

        private void UpdateHealth(int obj)
        {
            _health = _baseStats.GetStat(Stat.Health);
            UpdateHealthPercentage();
        }

        private void Start()
        {
           // _health = _baseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            _health -= damage;
            CheckDeathState();
            UpdateHealthPercentage();

            if (IsDead)
            {
               AwardExperience(instigator); 
            }
        }

        public void UpdateHealthPercentage()
        {
            if (_baseStats == null) return;

            float fullHealthAmount = _baseStats.GetStat(Stat.Health);
            float healthPercentage = _health / fullHealthAmount * 100;
            HealthChanged?.Invoke(healthPercentage);
        }

        private void AwardExperience(GameObject instigator)
        {
            float experienceReward = _baseStats.GetStat(Stat.ExperienceReward);
           
            if (instigator.TryGetComponent(out Experience experience))
            {
                experience.GainExperience(experienceReward);
            }
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