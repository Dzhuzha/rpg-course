using System;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventory
{
    public class ActionStore : MonoBehaviour, ISaveable
    {
        private Dictionary<int, DockedItemSlot> _dockedItems = new Dictionary<int, DockedItemSlot>();

        public event Action StoreUpdated;

        private class DockedItemSlot
        {
            public ActionItem Item;
            public int Number;
        }

        [Serializable]
        private struct DockedItemRecord
        {
            public string ItemID;
            public int Number;
        }

        public ActionItem GetAction(int index)
        {
            return _dockedItems.ContainsKey(index) ? _dockedItems[index].Item : null;
        }

        public int GetNumber(int index)
        {
            return _dockedItems.ContainsKey(index) ? _dockedItems[index].Number : 0;
        }

        /// <summary>
        /// Add an item to the given index.
        /// </summary>
        /// <param name="item">What item should be added.</param>
        /// <param name="index">Where item should be added.</param>
        /// <param name="number">How many of items should be added.</param>
        public void AddAction(InventoryItem item, int index, int number)
        {
            if (_dockedItems.ContainsKey(index))
            {
                if (ReferenceEquals(item, _dockedItems[index].Item))
                {
                    _dockedItems[index].Number += number;
                }
            }
            else
            {
                var slot = new DockedItemSlot();
                slot.Item = item as ActionItem;
                slot.Number = number;
                _dockedItems[index] = slot;
                // _dockedItems[index].Item = (ActionItem) item;
                // _dockedItems[index].Number = number;
            }

            StoreUpdated?.Invoke();
        }

        /// <summary>
        /// Removes given number of items from the given index.
        /// </summary>
        public void RemoveActions(int index, int number)
        {
            if (!_dockedItems.ContainsKey(index)) return;

            _dockedItems[index].Number -= number;
            if (_dockedItems[index].Number <= 0)
            {
                _dockedItems.Remove(index);
            }

            StoreUpdated?.Invoke();
        }

        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;
            
            if (actionItem == null) return 0;
            if (_dockedItems.ContainsKey(index) && !ReferenceEquals(item, _dockedItems[index].Item)) return 0; //actionItem
            if (actionItem.IsConsumable) return int.MaxValue;
            if (_dockedItems.ContainsKey(index)) return 0;

            return 1;
        }

        object ISaveable.CaptureState()
        {
           var state = new Dictionary<int, DockedItemRecord>();
         
           foreach (var pair in _dockedItems)
           {
               var record = new DockedItemRecord();
               record.ItemID = pair.Value.Item.ItemID;
               record.Number = pair.Value.Number;
               state[pair.Key] = record;
           }
           
           return state;
        }

        void ISaveable.RestoreState(object state)
        {
            var deserializedState = (Dictionary<int, DockedItemRecord>) state;

            foreach (var pair in deserializedState)
            {
                AddAction(InventoryItem.GetFromID(pair.Value.ItemID), pair.Key, pair.Value.Number);
            }
        }
    }
}