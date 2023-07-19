using UnityEngine;

namespace RPG.Inventory
{
    /// <summary>
    /// An inventory item that can be placed in the action bar and "Used".
    /// It's a base realisation. Subclasses must implement the `Use` method.
    /// </summary>
    [CreateAssetMenu(menuName = ("Inventory/Action Item"), fileName = "ActionItem", order = 2)]
    public class ActionItem : InventoryItem
    {
        [Tooltip("Does an instance of this item get consumed every time it's used."), SerializeField] private bool _consumable = false;

        public bool IsConsumable => _consumable;

        public virtual void Use(GameObject user)
        {
            Debug.Log("Using action: " + this);
        }
    }
}