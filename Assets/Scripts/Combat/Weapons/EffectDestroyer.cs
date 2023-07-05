using UnityEngine;

namespace RPG.Core
{
    public class EffectDestroyer : MonoBehaviour
    {
        [SerializeField] private GameObject _targetToDestroy;
        private ParticleSystem _effectToDestroy;

        private void Start()
        {
            _effectToDestroy = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (_effectToDestroy == null || _effectToDestroy.IsAlive()) return;
            
            if (_targetToDestroy != null)
            {
                Destroy(_targetToDestroy);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}