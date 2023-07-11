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
        [SerializeField] private AudioSource _audioSource;
        
        private AudioClip _hitAudioClip;
        private Health _target;
        private GameObject _instigator;
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

        public void InitArrow(Health target, GameObject instigator, float damage, AudioClip launchAudioClip = null, AudioClip hitAudioClip = null)
        {
            _target = target;
            _damage = damage;
            _instigator = instigator;
            
            _audioSource.PlayOneShot(launchAudioClip);
            
            if (hitAudioClip != null)
            {
                _hitAudioClip = hitAudioClip;
            }

            Destroy(gameObject, _maxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Health targetHealth) && targetHealth == _target &&
                targetHealth.IsDead == false)
            {
                _audioSource.PlayOneShot(_hitAudioClip);
                targetHealth.TakeDamage(_instigator, _damage);

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