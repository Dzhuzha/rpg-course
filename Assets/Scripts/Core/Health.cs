using RPG.Saving;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _health = 100f;

        private ActionScheduler _actionScheduler;

        public bool IsDead => _health <= 0;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            CheckDeathState();
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
            if (!IsDead) return;
            ForceDead();
        }
    }
}