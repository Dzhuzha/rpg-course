using System;
using System.Collections.Generic;
using RPG.Core;
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
    public class Equipment : MonoBehaviour, ISaveable, IPredicateEvaluator
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
        
        public IEnumerable<EquipLocation> GetAllEquippedItems()
        {
            return _equippedItems.Keys;
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

        public bool? Evaluate(PredicateType predicate, string[] parameters)
        {
            if (predicate == PredicateType.HasItemEquipped)
            {
                foreach (EquipableItem item in _equippedItems.Values)
                {
                    if (item.ItemID == parameters[0])
                    {
                        return true;
                    }
                }

                return false;
            }

            return null;
        }
    }
}