using RPG.Inventory;
using RPG.UI.Utils;
using UnityEngine;

namespace RPG.UI.Inventory
{
    public class InventorySlotUI : MonoBehaviour, IDragContainer<InventoryItem>, IItemHolder
    {
        [SerializeField] InventoryItemIcon _icon;
        
        private RPG.Inventory.Inventory _inventory;
        private int _index;

        public void Setup(RPG.Inventory.Inventory inventory, int index)
        {
            _inventory = inventory;
            _index = index;
            _icon.SetItem(inventory.GetItemInSlot(index), inventory.GetItemCountInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (_inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }

            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            _inventory.AddItemToSlot(_index, item, number);
        }

        public InventoryItem GetItem()
        {
            return _inventory.GetItemInSlot(_index);
        }

        public int GetNumber()
        {
            return _inventory.GetItemCountInSlot(_index);
        }

        public void RemoveItems(int count)
        {
            _inventory.RemoveFromSlot(_index, count);
        }
    }
}