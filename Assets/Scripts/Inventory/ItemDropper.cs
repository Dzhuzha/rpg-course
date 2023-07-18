using System;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

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

        // PUBLIC

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup</param>
        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation());
        }

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location where the drop should be spawned</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        private void SpawnPickup(InventoryItem item, Vector3 spawnLocation)
        {
            var pickup = item.SpawnPickup(spawnLocation);
            _droppedItems.Add(pickup);
        }

        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            _droppedItems.RemoveAll(pickup => pickup == null);

          // List<Pickup> newPickups = new List<Pickup>();
          // foreach (Pickup pickup in _droppedItems)
          // {
          //     if (pickup != null)
          //     {
          //         newPickups.Add(pickup);
          //     }
          // }
          // _droppedItems = newPickups;
        }

        object ISaveable.CaptureState()
        {
            RemoveDestroyedDrops();
            DropRecord[] droppedItemsList = new DropRecord[_droppedItems.Count];
            for (int i = 0; i < droppedItemsList.Length; i++)
            {
                droppedItemsList[i].ItemId = _droppedItems[i].Item.ItemID;
                droppedItemsList[i].Position = new SerializableVector3(_droppedItems[i].transform.position);
            }

            return droppedItemsList;
        }

        void ISaveable.RestoreState(object state)
        {
            DropRecord[] droppedItemsList = (DropRecord[]) state;
            foreach (DropRecord item in droppedItemsList)
            {
                var pickup = InventoryItem.GetFromID(item.ItemId);
                Vector3 position = item.Position.ToVector();
                SpawnPickup(pickup, position); 
            }
        }

        [Serializable]
        private struct DropRecord
        {
            public string ItemId;
            public SerializableVector3 Position;
        }
    }
}