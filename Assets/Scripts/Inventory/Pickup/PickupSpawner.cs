using RPG.Saving;
using UnityEngine;

namespace RPG.Inventory
{
    /// <summary>
    /// Spawn pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        // CONFIG DATA
        [SerializeField] InventoryItem _item;
        [SerializeField] private int _itemCount = 1;

        private void Awake()
        {
            // Spawn in Awake so can be destroyed by save system after.
            SpawnPickup();
        }

        // PUBLIC

        /// <summary>
        /// Returns the pickup spawned by this class if it exists.
        /// </summary>
        /// <returns>Returns null if pickup has been collected.</returns>
        public Pickup GetPickup()
        {
            return GetComponentInChildren<Pickup>();
        }

        /// <summary>
        /// True if the pickup was collected.
        /// </summary>
        /// <returns></returns>
        public bool IsCollected()
        {
            return GetPickup() == null;
        }

        // PRIVATE
        private void SpawnPickup()
        {
            var spawnedPickup = _item.SpawnPickup(transform.position, _itemCount);
            spawnedPickup.transform.SetParent(transform);
        }

        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }

        object ISaveable.CaptureState()
        {
            return IsCollected();
        }

        void ISaveable.RestoreState(object state)
        {
            bool shouldBeCollected = (bool)state;

            if (shouldBeCollected && !IsCollected())
            {
                DestroyPickup();
            }
            
            if (!shouldBeCollected && IsCollected())
            {
                SpawnPickup();
            }
        }
    }
}