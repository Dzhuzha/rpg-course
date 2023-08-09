using System;
using RPG.Core;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventory
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of slots are available.
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Inventory : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [Tooltip("Allowed size"), SerializeField] private int _inventorySize = 16;
        private InventorySlot[] _slots;

        public event Action InventoryUpdated;

        /// <summary>
        /// Convenience for getting the player's inventory.
        /// </summary>
        /// <returns></returns>
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player");
            return player.GetComponent<Inventory>();
        }

        /// <summary>
        /// Could this item fit anywhere in the inventory?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }
        
        public int GetSize()
        {
            return _slots.Length;
        }

        public bool AddToFirstEmptySlot(InventoryItem item, int itemCount)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }
            
            _slots[i].Item = item;
            _slots[i].Quantity += itemCount;
            InventoryUpdated?.Invoke();
            return true;
        }

        /// <summary>
        /// Is there an instance of this item in the inventory?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (ReferenceEquals(_slots[i].Item, item))
                {
                    return true;
                }
            }

            return false;
        }
        
        public InventoryItem GetItemInSlot(int slot)
        {
            return _slots[slot].Item;
        }
        
        public int GetItemCountInSlot(int slot)
        {
            return _slots[slot].Quantity;
        }

        /// <summary>
        /// Remove the item from the given slot.
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveFromSlot(int slot, int count)
        {
            _slots[slot].Quantity -= count;

            if (_slots[slot].Quantity <= 0)
            {
                _slots[slot].Quantity = 0;
                _slots[slot].Item = null;
            }
            
            InventoryUpdated?.Invoke();
        }

        /// <summary>
        /// Will add an item to the given slot if possible.
        /// If there is already a stack of this type in the slot,
        /// it will add to that stack. Otherwise, it will try to add to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <param name="count">The number of items to add.</param>
        /// <returns>True if the item was added anywhere in the inventory</returns>
        public bool AddItemToSlot(int slot, InventoryItem item, int count)
        {
            if (_slots[slot].Item != null)
            {
                return AddToFirstEmptySlot(item, count);
            }

            int index = FindStack(item); 
            if (index >= 0)
            {
                slot = index;
            }

            _slots[slot].Item = item;
            _slots[slot].Quantity += count;
            InventoryUpdated?.Invoke();
            return true;
        }

        //PRIVATE

        private void Awake()
        {
            _slots = new InventorySlot[_inventorySize];
        }

        /// <summary>
        /// Find a slot that can accomodate this item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int FindSlot(InventoryItem item)
        {
            int index = FindStack(item);

            if (index < 0)
            {
                index = FindEmptySlot();
            }
            
            return index;
        }

        
        /// <summary>
        /// Searching for a stack of items with the same type and id.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>-1 if no stack exists or item is not stackable</returns>
        private int FindStack(InventoryItem item)
        {
            if (item == null || !item.IsStackable) return -1;

            for (int i = 0; i < _slots.Length; i++)
            {
                // if (_slots[i].Item == null) continue;
                //
                // if (_slots[i].Item.ItemID == item.ItemID)
                // {
                //     return i;
                // }

                if (ReferenceEquals(_slots[i].Item, item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Find the first empty slot in the inventory.
        /// </summary>
        /// <returns></returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].Item == null)
                {
                    return i;
                }
            }

            return -1;
        }
        
        object ISaveable.CaptureState()
        {
            InventorySlotRecord[] slotsStrings = new InventorySlotRecord[_inventorySize];
            for (int i = 0; i < _inventorySize; i++)
            {
                if (_slots[i].Item != null)
                {
                    slotsStrings[i].ItemId = _slots[i].Item.ItemID;
                    slotsStrings[i].Quantity = _slots[i].Quantity;
                }
            }

            return slotsStrings;
        }

        void ISaveable.RestoreState(object state)
        {
            InventorySlotRecord[] slotsStrings = (InventorySlotRecord[])state;
            for (int i = 0; i < _inventorySize; i++)
            {
                if (i >= _inventorySize)
                {
                    Debug.LogWarning("Saved inventory state does not match inventory size.");
                    break;
                }
                
                _slots[i].Item = InventoryItem.GetFromID(slotsStrings[i].ItemId);
                _slots[i].Quantity = slotsStrings[i].Quantity;
            }

            InventoryUpdated?.Invoke();
        }

        [Serializable]
        private struct InventorySlotRecord
        {
            public string ItemId;
            public int Quantity;
        }

        private struct InventorySlot
        {
            public InventoryItem Item;
            public int Quantity;
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "HasInventoryItem":
                    return HasItem(InventoryItem.GetFromID(parameters[0]));
                case "HasNoItem":
                    return !HasItem(InventoryItem.GetFromID(parameters[0]));
                default:
                    return null;
            }
        }
    }
}