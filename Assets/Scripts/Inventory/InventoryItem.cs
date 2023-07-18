using System;
using System.Collections.Generic;
using System.Linq;
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
        [Tooltip("The prefab that should be spawned when this item is dropped.")]
        [SerializeField] private Pickup _pickup = null;
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
        
        /// <summary>
        /// Spawn the pickup game object into the scene.
        /// </summary>
        /// <param name="position">Where to spawn the pickup.</param>
        /// <returns>Reference to pickup object spawned.</returns>
        public Pickup SpawnPickup(Vector3 position, int count)
        {
            Pickup pickup = Instantiate(_pickup, position, Quaternion.identity);
            pickup.Setup(this, count);
            return pickup;
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(_itemID))
            {
                _itemID = Guid.NewGuid().ToString();
            }
            
            // // Test for multiple objects with the same UUID
            // var items = Resources.LoadAll<InventoryItem>(""). //continues below
            //     Where(p => p.ItemID == _itemID).ToList();
            // if (items.Count > 1)
            // {
            //     _itemID = Guid.NewGuid().ToString();
            // }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }
    }
}