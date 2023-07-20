using UnityEngine;

namespace RPG.Inventory
{
    [CreateAssetMenu(menuName = ("Inventory/Storable Item"), fileName = "StorableItem", order = 2)]
    public class StorableItem : InventoryItem
    {
        [Tooltip("Does an instance of this item get consumed every time it's used."), SerializeField] private bool _consumable = false;

        public bool IsConsumable => _consumable;
    }
}