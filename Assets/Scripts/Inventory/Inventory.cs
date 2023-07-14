using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventory
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of slots are available.
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Inventory : MonoBehaviour, ISaveable
    {
        // CONFIG DATA
        [Tooltip("Allowed size"), SerializeField]
        private int _inventorySize = 16;

        // STATE
        private InventoryItem[] _slots;

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
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

        /// <summary>
        /// How many slots are in the inventory?
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return _slots.Length;
        }

        private bool AddToFirstEmptySlot(InventoryItem item)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }

            _slots[i] = item;
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
                if (ReferenceEquals(_slots[i], item))
                {
                    return true;
                }
            }

            return false;
            // foreach (var slot in _slots)
            // {
            //     if (slot == item)
            //     {
            //         return true;
            //     }
            // }
            //
            // return false;
        }

        /// <summary>
        /// Return the item in the given slot.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public InventoryItem GetItemInSlot(int slot)
        {
            return _slots[slot];
        }

        /// <summary>
        /// Remove the item from the given slot.
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveFromSlot(int slot)
        {
            _slots[slot] = null;
            InventoryUpdated?.Invoke();
        }

        /// <summary>
        /// Will add an item to the given slot if possible.
        /// If there is already a stack of this type in the slot,
        /// it will add to that stack. Otherwise, it will try to add to the first empty slot.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="item"></param>
        /// <returns>True if the item was added anywhere in the inventory</returns>
        public bool AddItemToSlot(int slot, InventoryItem item)
        {
            if (_slots[slot] != null)
            {
                return AddToFirstEmptySlot(item);
            }

            _slots[slot] = item;
            InventoryUpdated?.Invoke();
            return true;
        }

        //PRIVATE

        private void Awake()
        {
            _slots = new InventoryItem[_inventorySize];
            _slots[0] = InventoryItem.GetFromID("91057185-d1d3-4ad7-92fa-cec8715634e5");
            _slots[1] = InventoryItem.GetFromID("090bd94a-6554-480f-9329-828ec6074e6e");
        }

        /// <summary>
        /// Find a slot that can accomodate this item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int FindSlot(InventoryItem item)
        {
            return FindEmptySlot();
        }

        /// <summary>
        /// Find the first empty slot in the inventory.
        /// </summary>
        /// <returns></returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }
        
        object ISaveable.CaptureState()
        {
            var slotsStrings = new string[_inventorySize];
            for (int i = 0; i < _inventorySize; i++)
            {
                if (_slots[i] != null)
                {
                    slotsStrings[i] = _slots[i].ItemID;
                }
            }

            return slotsStrings;
        }

        void ISaveable.RestoreState(object state)
        {
            var slotsStrings = (string[])state;
            for (int i = 0; i < _inventorySize; i++)
            {
                if (i >= _inventorySize)
                {
                    Debug.LogWarning("Saved inventory state does not match inventory size.");
                    break;
                }
                
                _slots[i] = InventoryItem.GetFromID(slotsStrings[i]);
            }

            InventoryUpdated?.Invoke();
        }
    }
}