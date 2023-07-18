using RPG.Inventory;
using TMPro;
using UnityEngine;

namespace RPG.UI.Inventory
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] TMP_Text _titleText;
        [SerializeField] TMP_Text _bodyText;

        public void SetUp(InventoryItem item)
        {
            _titleText.text = item.DisplayName;
            _bodyText.text = item.Description;
        }
    }
}