using UnityEngine;

namespace RPG.Inventory
{
    [CreateAssetMenu(menuName = ("Inventory/Equipable Item"), fileName = "Equipable", order = 0)]
    public class EquipableItem : InventoryItem
    {
        [Tooltip("Where are we allowed to put this item."), SerializeField] EquipLocation _allowedEquipLocation = EquipLocation.MainHand;

        public EquipLocation AllowedEquipLocation => _allowedEquipLocation;
    }
}