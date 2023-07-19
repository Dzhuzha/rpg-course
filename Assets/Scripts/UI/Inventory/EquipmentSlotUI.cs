using RPG.Inventory;
using RPG.UI.Utils;
using UnityEngine;

namespace RPG.UI.Inventory
{
    public class EquipmentSlotUI : MonoBehaviour, IDragContainer<InventoryItem>, IItemHolder
    {
        [SerializeField] InventoryItemIcon _icon;
        [SerializeField] EquipLocation _equipLocation = EquipLocation.MainHand;

        private Equipment _playerEquipment;

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _playerEquipment = player.GetComponent<Equipment>();
            _playerEquipment.EquipmentUpdated += RedrawUI;
        }

        private void Start()
        {
            RedrawUI();
        }
        
        private void OnDestroy()
        {
            _playerEquipment.EquipmentUpdated -= RedrawUI;
        }

        private void RedrawUI()
        {
            _icon.SetItem(_playerEquipment.GetItemInSlot(_equipLocation));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;
            if (equipableItem == null) return 0;
            if (equipableItem.AllowedEquipLocation != _equipLocation) return 0;
            if (GetItem() != null) return 0;
            
            return 1;
        }

        public void AddItems(InventoryItem item, int number)
        {
           _playerEquipment.AddItem(_equipLocation, (EquipableItem) item);
        }

        public InventoryItem GetItem()
        {
            return _playerEquipment.GetItemInSlot(_equipLocation);
        }

        public int GetNumber()
        {
            return GetItem() != null? 1 : 0;
        }

        public void RemoveItems(int number)
        {
           _playerEquipment.RemoveItem(_equipLocation);
        }
    }
}