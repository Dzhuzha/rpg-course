using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.UI.Utils
{
    /// <summary>
    /// Allows a UI element to be dragged and dropped from and to other a container.
    ///
    ///Create a subclass for the type you want to be draggable.
    /// Then place this on the UI element you want to make draggable.
    ///
    /// During dragging, the item is reparented to the parent canvas.
    ///
    /// After the item is dropped, it is automatically return to the original UI parent.
    /// It is the job of components implementing IDragContainer, IDragDestination
    /// and IDragSource to update the interface after drag has occured.
    /// </summary>
    /// <typeparam name="T">The type that represents the item to be dragged</typeparam>
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : class
    {
        // Private state
        private Vector3 _startPosition;
        private Transform _originalParent;
        private IDragSource<T> _source;

        // Cached references
        private Canvas _parentCanvas;

        // Lifecycle methods
        private void Awake()
        {
            _parentCanvas = GetComponentInParent<Canvas>();
            _source = GetComponentInParent<IDragSource<T>>();
        }

        // Private
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = transform.position;
            _originalParent = transform.parent;
            // Else won't get the drop event.
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(_parentCanvas.transform, true);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            transform.position = _startPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(_originalParent, true);

            IDragDestination<T> container;
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                container = _parentCanvas.GetComponent<IDragDestination<T>>();
            }
            else
            {
                container = GetContainer(eventData);
            }

            if (container != null)
            {
                DropItemIntoContainer(container);
            }
        }

        private IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
                return container;
            }

            return null;
        }

        private void DropItemIntoContainer(IDragDestination<T> destination)
        {
            if (object.ReferenceEquals(destination, _source)) return;

            var destinationContainer = destination as IDragContainer<T>;
            var sourceContainer = _source as IDragContainer<T>;

            // Swap won't possible if either container is null.
            if (destinationContainer == null || sourceContainer == null || destinationContainer.GetItem() == null
                || object.ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
            {
                AttemptSimpleTransfer(destination);
                return;
            }

            AttemptSwap(destinationContainer, sourceContainer);
        }

        private void AttemptSwap(IDragContainer<T> destinationContainer, IDragContainer<T> sourceContainer)
        {
            // Temporarily remove items from both containers
            var removedSourceNumber = sourceContainer.GetNumber();
            var removedSourceItem = sourceContainer.GetItem();
            var removedDestinationNumber = destinationContainer.GetNumber();
            var removedDestinationItem = destinationContainer.GetItem();

            sourceContainer.RemoveItems(removedSourceNumber);
            destinationContainer.RemoveItems(removedDestinationNumber);

            var sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, sourceContainer,
                destinationContainer);
            var destinationTakeBackNumber = CalculateTakeBack(removedDestinationItem, removedDestinationNumber,
                destinationContainer, sourceContainer);

            // Do take backs if necessary
            if (sourceTakeBackNumber > 0)
            {
                sourceContainer.AddItems(removedSourceItem, sourceTakeBackNumber);
                removedSourceNumber -= sourceTakeBackNumber;
            }

            if (destinationTakeBackNumber > 0)
            {
                destinationContainer.AddItems(removedDestinationItem, destinationTakeBackNumber);
                removedDestinationNumber -= destinationTakeBackNumber;
            }

            // Abort if we can't do the swap
            if (sourceContainer.MaxAcceptable(removedSourceItem) < removedDestinationNumber ||
                destinationContainer.MaxAcceptable(removedDestinationItem) < removedSourceNumber)
            {
                destinationContainer.AddItems(removedDestinationItem, removedDestinationNumber);
                sourceContainer.AddItems(removedSourceItem, removedSourceNumber);
                return;
            }

            // Do the swap
            if (removedDestinationNumber > 0)
            {
                sourceContainer.AddItems(removedDestinationItem, removedDestinationNumber);
            }

            if (removedSourceNumber > 0)
            {
                destinationContainer.AddItems(removedSourceItem, removedSourceNumber);
            }
        }

        private bool AttemptSimpleTransfer(IDragDestination<T> destination)
        {
            var draggingItem = _source.GetItem();
            var draggingAmount = _source.GetNumber();

            var acceptable = destination.MaxAcceptable(draggingItem);
            var toTransfer = Mathf.Min(acceptable, draggingAmount);

            if (toTransfer > 0)
            {
                _source.RemoveItems(toTransfer);
                destination.AddItems(draggingItem, toTransfer);
                return false;
            }

            return true;
        }

        private int CalculateTakeBack(T removedItem, int removedAmount, IDragContainer<T> removedFrom, IDragContainer<T> destination)
        {
            var takeBackNumber = 0;
            var destinationMaxAcceptable = destination.MaxAcceptable(removedItem);

            if (destinationMaxAcceptable < removedAmount)
            {
                takeBackNumber = removedAmount - destinationMaxAcceptable;

                var sourceTakeBackAcceptable = removedFrom.MaxAcceptable(removedItem);

                //Abort and reset if the source can't take back the number of items we want to take back.
                if (sourceTakeBackAcceptable < takeBackNumber)
                {
                    return 0;
                }
            }

            return takeBackNumber;
        }
    }
}