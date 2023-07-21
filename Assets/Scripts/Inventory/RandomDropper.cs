using RPG.Atributes;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RPG.Inventory
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far can the pickups be scattered from the dropper."), SerializeField]
        private float _distance = 1f;

        [SerializeField] private DropLibrary _dropLibrary;
        [SerializeField] private Health _health;

        private const int ATTEMPTS = 30;

        private void OnEnable()
        {
            if (_health != null)
            {
                _health.Dead += DropRandom;
            }
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.Dead -= DropRandom;
            }
        }

        private void DropRandom()
        {
            var baseStats = GetComponent<BaseStats>();
            int dropperLevel = baseStats.CurrentLevel.value;
            var drops = _dropLibrary.GetRandomDrops(dropperLevel);

            foreach (var drop in drops)
            {
                DropItem(drop.Item, drop.Quantity);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPTS; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * _distance;

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }
    }
}