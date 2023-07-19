using RPG.Inventory;
using RPG.UI.Utils;
using UnityEngine;

namespace RPG.UI.Inventory
{
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] private InventoryItemIcon _icon;
        [SerializeField] private int _index;

        private ActionStore _actionStore;

        private void Awake()
        {
           _actionStore = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionStore>();
           _actionStore.StoreUpdated += RedrawUI;
        }

        private void OnDestroy()
        {
            _actionStore.StoreUpdated -= RedrawUI;
        }

        private void RedrawUI()
        {
           _icon.SetItem(GetItem(), GetNumber());
        }

        public InventoryItem GetItem()
        {
            return _actionStore.GetAction(_index);
        }

        // InventoryItem IItemHolder.GetItem()
        // {
        //     return _actionStore.GetAction(_index);
        // }

        public int GetNumber()
        {
           return _actionStore.GetNumber(_index);
        }

        public void RemoveItems(int number)
        {
           _actionStore.RemoveActions(_index, number);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return _actionStore.MaxAcceptable(item, _index);
        }

        public void AddItems(InventoryItem item, int number)
        {
            _actionStore.AddAction(item, _index, number);
        }

        // InventoryItem IDragSource<InventoryItem>.GetItem()
        // {
        //     throw new NotImplementedException();
        // }
    }
}