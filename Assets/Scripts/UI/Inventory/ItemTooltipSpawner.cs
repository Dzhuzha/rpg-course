using RPG.UI.Utils.Tooltips;
using UnityEngine;

namespace RPG.UI.Inventory
{
    /// <summary>
    /// To be placed on a UI slot to spawn and show the correct item tooltip.
    /// </summary>
    
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        public override void UpdateTooltip(GameObject tooltip)
        {
            var itemTooltip = tooltip.GetComponent<ItemTooltip>();
            if (!itemTooltip) return;
    
            var item = GetComponentInParent<IItemHolder>().GetItem();
            itemTooltip.SetUp(item);
        }

        public override bool CanCreateTooltip()
        {
           var item = GetComponentInParent<IItemHolder>().GetItem();
           if (!item) return false;

           return true;
        }
    }
}