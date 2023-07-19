using System;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventory
{
    /// <summary>
    /// Provides a store for the equipment that the player is wearing.
    /// Items are stored in the same order as the EquipLocation enum.
    ///
    /// Should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Equipment : MonoBehaviour, ISaveable
    {
        private Dictionary<EquipLocation, EquipableItem> _equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        public event Action EquipmentUpdated;
        
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
           return _equippedItems.ContainsKey(equipLocation) ? _equippedItems[equipLocation] : null;
        }

        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            //Debug.Assert(item.AllowedEquipLocation == slot);
            _equippedItems[slot] = item;
            EquipmentUpdated?.Invoke();
        }

        public void RemoveItem(EquipLocation slot)
        {
            _equippedItems.Remove(slot);
            EquipmentUpdated?.Invoke();
        }

        object ISaveable.CaptureState()
        {
            Dictionary<EquipLocation, string> equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in _equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.ItemID;
            }

            return equippedItemsForSerialization;
        }

        void ISaveable.RestoreState(object state)
        {
            _equippedItems = new Dictionary<EquipLocation, EquipableItem>();
            
            Dictionary<EquipLocation, string> equippedItemsForSerialization = (Dictionary<EquipLocation, string>) state;

            foreach (var pair in equippedItemsForSerialization)
            {
                EquipableItem item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    _equippedItems[pair.Key] = item;
                }
            }
        }
    }
}