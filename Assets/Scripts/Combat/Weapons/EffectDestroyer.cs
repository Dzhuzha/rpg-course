using UnityEngine;

namespace RPG.Core
{
    public class EffectDestroyer : MonoBehaviour
    {
        private ParticleSystem _effectToDestroy;

        private void Start()
        {
            _effectToDestroy = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (_effectToDestroy != null && !_effectToDestroy.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}