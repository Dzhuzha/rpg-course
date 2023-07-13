using UnityEngine;

namespace RPG.UI.Inventory
{
    /// <summary>
    /// To be placed on the root inventory UI object.
    /// Handles spawning all the inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] InventorySlotUI _inventoryItemPrefab = null;
        RPG.Inventory.Inventory _playerInventory;

        private void Awake()
        {
            _playerInventory = RPG.Inventory.Inventory.GetPlayerInventory();
            _playerInventory.InventoryUpdated += Redraw;
        }

        private void OnDestroy()
        {
            _playerInventory.InventoryUpdated -= Redraw;
        }

        private void Start()
        {
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < _playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(_inventoryItemPrefab, transform);
                itemUI.Setup(_playerInventory, i);
            }
        }
    }
}