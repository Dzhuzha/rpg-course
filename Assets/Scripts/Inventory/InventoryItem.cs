using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventory
{
    /// <summary>
    /// A scriptable object representing any item that can be put in inventory.
    /// In practice, likely to use a subclass such as `ActionItem` or EquipableItem.
    /// </summary>
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "Inventory/InventoryItem", order = 51)]
    public class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        // CONFIG DATA
        [Tooltip("Unique ID for each item. Clear this field to generate a new ID.")] 
        [SerializeField] private string _itemID = String.Empty;
        [Tooltip("The name to use for this item when it appears in the UI.")] 
        [SerializeField] private string _displayName = String.Empty;
        [Tooltip("The description to use for this item when it appears in the UI.")] 
        [SerializeField, TextArea] private string _description = String.Empty;
        [Tooltip("The UI icon to represent this item in the inventory.")] 
        [SerializeField] private Sprite _icon = null;
        [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")] 
        [SerializeField] private bool _stackable = false;
        
        public Sprite Icon => _icon;
        public string ItemID => _itemID;
        public string DisplayName => _displayName;
        public string Description => _description;
        public bool IsStackable => _stackable;
        
        // STATE
        static Dictionary<string, InventoryItem> _itemLookupCache;
        
        // PUBLIC

        public static InventoryItem GetFromID(string itemID)
        {
            if (_itemLookupCache == null)
            {
                _itemLookupCache = new Dictionary<string, InventoryItem>();
                var itemList = Resources.LoadAll<InventoryItem>("");
                foreach (var item in itemList)
                {
                    if (_itemLookupCache.ContainsKey(item._itemID))
                    {
                        Debug.LogError($"Looks like there's a duplicate InventoryItem ID for objects: {_itemLookupCache[item._itemID]} and {item}");
                        continue;
                    }

                    _itemLookupCache[item._itemID] = item;
                }
            }

            if (itemID == null || !_itemLookupCache.ContainsKey(itemID)) return null;
            return _itemLookupCache[itemID];
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(_itemID))
            {
                _itemID = Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }
    }
}