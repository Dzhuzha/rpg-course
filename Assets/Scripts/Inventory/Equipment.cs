using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventory
{
    public class Equipment : MonoBehaviour, IJsonSaveable
    {
        private Dictionary<EquipLocation, EquipableItem> _equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        public event Action EquipmentUpdated;
        
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
           return _equippedItems.ContainsKey(equipLocation) ? _equippedItems[equipLocation] : null;
        }

        public void AddItem(EquipLocation slot, EquipableItem item)
        {
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

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDictionary = state;

            foreach (var pair in _equippedItems)
            {
                stateDictionary[pair.Key.ToString()] = JToken.FromObject(pair.Value.ItemID);
            }

            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                _equippedItems.Clear();
                IDictionary<string, JToken> stateDictionary = stateObject;

                foreach (var pair in stateObject)
                {
                    if (Enum.TryParse(pair.Key, out EquipLocation key))
                    {
                        if (InventoryItem.GetFromID(pair.Value.ToObject<string>()) is EquipableItem item)
                        {
                            _equippedItems[key] = item;
                        }
                    }
                }
                
                EquipmentUpdated?.Invoke();
            }
        }
    }
}