using RPG.Atributes;
using UnityEngine;

namespace RPG.Combat
{
    public class ProjectTile : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private bool IsHoming;
        [SerializeField] private GameObject _hitEffect;
        [SerializeField] private float _lifeAfterImpact = 2f;
        [SerializeField] private float _maxLifeTime = 10f;
        [SerializeField] private GameObject[] _objectsToDestroy;

        private Health _target;
        private Vector3 _targetPosition;
        private float _damage;

        private void Start()
        {
            if (_target == null) return;

            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (_target == null) return;

            Fly();
        }

        private void Fly()
        {
            transform.Translate(Vector3.forward * (_speed * Time.deltaTime));

            if (IsHoming == false || _target.IsDead) return;
            transform.LookAt(GetAimLocation());
        }

        private Vector3 GetAimLocation()
        {
            if (_target.TryGetComponent(out CapsuleCollider collider))
            {
                return _target.transform.position + Vector3.up * collider.height / 2;
            }

            return _target.transform.position;
        }

        public void InitArrow(Health target, float damage)
        {
            _target = target;
            _damage = damage;

            Destroy(gameObject, _maxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Health targetHealth) && targetHealth == _target &&
                targetHealth.IsDead == false)
            {
                targetHealth.TakeDamage(_damage);

                if (_hitEffect != null)
                {
                    Instantiate(_hitEffect, transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.Log($"No hit effect for {gameObject.name}");
                }

                foreach (var obj in _objectsToDestroy)
                {
                    Destroy(obj);
                }

                Destroy(gameObject, _lifeAfterImpact);
            }
        }
    }
}