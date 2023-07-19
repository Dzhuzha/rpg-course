using RPG.Inventory;
using TMPro;
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
        [SerializeField] private GameObject _quantityTextContainer;
        [SerializeField] private TMP_Text _quantity;

        public void SetItem(InventoryItem item)
        {
            //TODO: Check why 0 is a value here
            SetItem(item, 0);
        }
        
        public void SetItem(InventoryItem item, int count)
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

                if (item.IsStackable)
                {
                    _quantityTextContainer.SetActive(true);
                    _quantity.text = count.ToString();
                }
                else
                {
                    _quantityTextContainer.SetActive(false);
                }
            }
        }
    }
}