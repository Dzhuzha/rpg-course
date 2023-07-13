namespace RPG.UI.Utils
{
    /// <summary>
    /// Components that implement this interface can act as the source for dragging a DragItem.
    /// </summary>
    /// <typeparam name="T">The type represents the item being dragged</typeparam>
    public interface IDragSource<T> where T : class 
    {
        /// <summary>
        /// What item type currently resides in this source?
        /// </summary>
        /// <returns></returns>
        T GetItem();
        
        /// <summary>
        /// What is the quantity of items in this source?
        /// </summary>
        /// <returns></returns>
        int GetNumber();

        /// <summary>
        /// Remove a given number of items from the source.
        /// </summary>
        /// <param name="number">This should be never exceed the number returned by "GetNumber"</param>
        void RemoveItems(int number);
    }
}