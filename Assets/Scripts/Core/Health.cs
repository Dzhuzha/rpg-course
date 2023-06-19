using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
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
            float newHealth = _health - damage;
            _health = newHealth > 0 ? newHealth : 0;

            if (IsDead)
            {
                Die();
            }
        }

        private void Die()
        {
            _actionScheduler.CancelCurrentAction();
            _animator.SetTrigger("Death");
        }
    }
}