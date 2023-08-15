using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventory
{
    /// <summary>
    /// Spawn pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    public class PickupSpawner : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] InventoryItem _item;
        [SerializeField] private int _itemCount = 1;

        private void Awake()
        {
            SpawnPickup();
        }

        public Pickup GetPickup()
        {
            return GetComponentInChildren<Pickup>();
        }
        
        public bool IsCollected()
        {
            return GetPickup() == null;
        }
        
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

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(IsCollected());
        }

        public void RestoreFromJToken(JToken state)
        {
            bool isCollected = IsCollected();
            bool shouldBeCollected = state.ToObject<bool>();

            if (shouldBeCollected && !isCollected)
            {
                DestroyPickup();
            }

            if (!shouldBeCollected && isCollected)
            {
                SpawnPickup();
            }
        }
    }
}