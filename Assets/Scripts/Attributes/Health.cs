using System;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Stats;
using RPG.Saving;
using UnityEngine;
using RPG.Core;
using RPG.Inventory;
using RPG.UI.DamageText;

namespace RPG.Atributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private DamageTextSpawner _damageTextSpawner;
        [SerializeField] private Equipment _equipment;
        
        private LazyValue<float> _health;
        private ActionScheduler _actionScheduler;
        private BaseStats _baseStats;

        public bool IsDead => _health.value <= 0;

        public event Action<float, float> HealthChanged;
        public event Action Dead;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _baseStats = GetComponent<BaseStats>();
            _health = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            _health.ForceInit();
            HealthChanged?.Invoke(_health.value, _baseStats.GetStat(Stat.Health));
        }

        private float GetInitialHealth()
        {
            return _baseStats.GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            _baseStats.LevelChanged += UpdateHealth;

            if (_equipment == null) return;
            _equipment.EquipmentUpdated += UpdateHealthPercentage;
        }

        private void UnSubscribe()
        {
            _baseStats.LevelChanged -= UpdateHealth;
            
            if (_equipment == null) return;
            _equipment.EquipmentUpdated += UpdateHealthPercentage;
        }

        private void UpdateHealth(int level)
        {
            _health.value = _baseStats.GetStat(Stat.Health);
            UpdateHealthPercentage();
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
           // Debug.Log($"{gameObject.name} took {damage} damage from {instigator.name}.");
            _damageTextSpawner.Spawn(damage);
            _health.value -= damage;
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
            HealthChanged?.Invoke(_health.value, fullHealthAmount);
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
            _health.value = 0f;
        }

        private void Die()
        {
            Dead?.Invoke();
            _actionScheduler.CancelCurrentAction();
            _animator.SetTrigger("Death");
        }

        private void ForceDead()
        {
            _actionScheduler.CancelCurrentAction();
            _animator.SetTrigger("Dead");
        }

        public void Heal(float healthToRestore)
        {
           _health.value = Mathf.Min(_health.value + healthToRestore, _baseStats.GetStat(Stat.Health));
           UpdateHealthPercentage();
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_health.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            _health.value = state.ToObject<float>();
            UpdateHealthPercentage();
            if (!IsDead) return;
            ForceDead();
        }
    }
}