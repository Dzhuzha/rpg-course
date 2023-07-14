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
        public void Setup(InventoryItem item)
        {
            _item = item;
        }
        
        public void PickupItem()
        {
            bool foundSlot = _inventory.AddToFirstEmptySlot(_item);
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