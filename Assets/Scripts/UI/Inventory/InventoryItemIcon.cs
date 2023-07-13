using RPG.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Inventory
{
    /// <summary>
    /// To be put on the icon representing an inventory item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        public void SetItem(InventoryItem item)
        {
            var iconImage = GetComponent<Image>();
           
            if (item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.sprite = item.Icon;
                iconImage.enabled = true;
            }
        }

        public Sprite GetItem()
        {
           var iconImage = GetComponent<Image>();
           
           if (iconImage.enabled == false) return null;
           
           return iconImage.sprite;
        }
    }
}