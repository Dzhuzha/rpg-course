using RPG.Atributes;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RPG.Inventory
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far can the pickups be scattered from the dropper."), SerializeField] private float _distance = 1f;
        [SerializeField] private InventoryItem[] _dropLibrary;
        [SerializeField] private int _dropItemsQuantity = 3;
        [SerializeField] private int _stackableItemRange = 10;
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
            int dropQuantity = Random.Range(1, _dropItemsQuantity);
            int stackQuantity = 1;
            
            for (int i = 0; i < dropQuantity; i++)
            {
                int itemIndex = Random.Range(0, _dropLibrary.Length);

                if (_dropLibrary[itemIndex].IsStackable)
                {
                    stackQuantity = Random.Range(1, _stackableItemRange);
                }
                
                DropItem(_dropLibrary[itemIndex], stackQuantity);
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