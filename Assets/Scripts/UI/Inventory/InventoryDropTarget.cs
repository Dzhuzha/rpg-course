using RPG.Inventory;
using RPG.UI.Utils;
using UnityEngine;

namespace RPG.UI.Inventory
{
    /// <summary>
    /// Handles spawning pickups when item dropped into the world.
    ///
    /// Must be placed on the root canvas where items can be dragged. Will be
    /// if dropped over empty space.
    /// </summary>
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        public int MaxAcceptable(InventoryItem item)
        {
            return int.MaxValue;
        }

        public void AddItems(InventoryItem item, int number)
        {
           var player = GameObject.FindGameObjectWithTag("Player");
           player.GetComponent<ItemDropper>().DropItem(item);
        }
    }
}