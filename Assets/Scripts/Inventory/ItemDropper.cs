using System;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Inventory
{
    /// <summary>
    /// To be on anything that can drop items.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        // STATE
        private List<Pickup> _droppedItems = new List<Pickup>();
        private List<DropRecord> _otherSceneDroppedItems = new List<DropRecord>();

        // PUBLIC

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup</param>
        public void DropItem(InventoryItem item, int count)
        {
            SpawnPickup(item, count, GetDropLocation());
        }

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location where the drop should be spawned</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        private void SpawnPickup(InventoryItem item, int count, Vector3 spawnLocation)
        {
            var pickup = item.SpawnPickup(spawnLocation, count);
            _droppedItems.Add(pickup);
        }

        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            _droppedItems.RemoveAll(pickup => pickup == null);
        }

        object ISaveable.CaptureState()
        {
            RemoveDestroyedDrops();
            List<DropRecord> droppedItemsList = new List<DropRecord>();
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            foreach (Pickup pickup in _droppedItems)
            {
                var droppedItem = new DropRecord();
                droppedItem.ItemId = pickup.Item.ItemID;
                droppedItem.Position = new SerializableVector3(pickup.transform.position);
                droppedItem.Count = pickup.Count;
                droppedItem.SceneBuildIndex = sceneIndex;
                droppedItemsList.Add(droppedItem);
            }
            
            droppedItemsList.AddRange(_otherSceneDroppedItems);
            return droppedItemsList;
        }

        void ISaveable.RestoreState(object state)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            _otherSceneDroppedItems.Clear();
            List<DropRecord> droppedItemsList = (List<DropRecord>) state;
            
            foreach (DropRecord item in droppedItemsList)
            {
                if (item.SceneBuildIndex != sceneIndex)
                {
                   _otherSceneDroppedItems.Add(item);
                   continue;
                }
                
                var pickup = InventoryItem.GetFromID(item.ItemId);
                Vector3 position = item.Position.ToVector();
                int count = item.Count;
                SpawnPickup(pickup, count, position); 
            }
        }

        [Serializable]
        private struct DropRecord
        {
            public int SceneBuildIndex;
            public string ItemId;
            public SerializableVector3 Position;
            public int Count;
        }
    }
}