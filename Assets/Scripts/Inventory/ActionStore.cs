using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventory
{
    public class ActionStore : MonoBehaviour, IJsonSaveable
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

        public bool Use(int index, GameObject user)
        {
            if (_dockedItems.ContainsKey(index))
            {
                _dockedItems[index].Item.Use(user);
                if (_dockedItems[index].Item.IsConsumable)
                {
                    RemoveActions(index, 1);
                }

                return true;
            }

            return false;
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
        
        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDictionary = state;

            foreach (var pair in _dockedItems)
            {
                JObject dockedState = new JObject();
                IDictionary<string, JToken> dockedStateDictionary = dockedState;
                dockedStateDictionary["item"] = JToken.FromObject(pair.Value.Item.ItemID);
                dockedStateDictionary["number"] = JToken.FromObject(pair.Value.Number);
                stateDictionary[pair.Key.ToString()] = dockedState;
            }

            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                IDictionary<string, JToken> stateDictionary = stateObject;

                foreach (var pair in stateDictionary)
                {
                    if (pair.Value is JObject dockedState)
                    {
                        int key = Int32.Parse(pair.Key);
                        IDictionary<string, JToken> dockedStateDictionary = dockedState;
                        var item = InventoryItem.GetFromID(dockedStateDictionary["item"].ToObject<string>());
                        int number = dockedStateDictionary["number"].ToObject<int>();
                        AddAction(item, key, number);
                    }
                }
            }
        }
    }
}