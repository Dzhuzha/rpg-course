using System;
using RPG.UI.Utils;
using UnityEngine;

namespace RPG.UI.Inventory
{
    public class InventorySlotUI : MonoBehaviour, IDragContainer<Sprite>
    {
        [SerializeField] InventoryItemIcon _icon;

        public int MaxAcceptable(Sprite item)
        {
            if (GetItem() == null)
            {
                return Int32.MaxValue;
            }

            return 0;
        }

        public Sprite GetItem()
        {
            return _icon.GetItem();
        }

        public void AddItems(Sprite item, int number)
        {
            _icon.SetItem(item);
        }

        public int GetNumber()
        {
            return 1;
        }

        public void RemoveItems(int number)
        {
            _icon.SetItem(null);
        }
    }
}