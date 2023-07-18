using UnityEngine;

namespace RPG.Inventory
{
    /// <summary>
    /// To be placed at the toor of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        // STATE
        InventoryItem _item;
        int _count = 1;
        
        public InventoryItem Item => _item;

        // CACHED REFERENCE
        Inventory _inventory;

        // LIFECYCLE METHODS

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _inventory = player.GetComponent<Inventory>();
        }
        
        // PUBLIC
        
        /// <summary>
        /// Set the vital data after creating the prefab
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        public void Setup(InventoryItem item, int itemCount)
        {
            _item = item;
            _count = itemCount;
        }
        
        public void PickupItem()
        {
            bool foundSlot = _inventory.AddToFirstEmptySlot(_item, _count);
            if (foundSlot)
            {
                Destroy(gameObject);
            }
        }
        
        public bool CanBePickedUp()
        {
            return _inventory.HasSpaceFor(_item);
        }
    }
}