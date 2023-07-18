using UnityEngine;

public class ExpandableScrollView : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private int _minColumns = 1;
    [SerializeField] private int _maxColumns = 6;
    [SerializeField] private int _floatExpandValue = 106;
    [SerializeField] private int _rowsCount = 4;
    [SerializeField] private float _minHeight = 220f;
    [SerializeField] private float _maxHeight = 730f;

    private void OnEnable()
    {
        ExpandScrollView();
    }

    /// <summary>
    /// This method is used to expand the scroll view based on the number of elements in the content.
    /// </summary>
    /// <param name="elementsCount"></param>
    private void ExpandScrollView()
    {
        int elementsCount = _content.transform.childCount;
        int itemsOutColumn = elementsCount % _rowsCount;
        int fullFilledColumns = elementsCount / _rowsCount;
        int columnsCount = fullFilledColumns + (itemsOutColumn > 0 ? 1 : 0);

        if (columnsCount > _minColumns && columnsCount < _maxColumns)
        {
            float expandValue = _floatExpandValue * (columnsCount - _minColumns);
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _minHeight + expandValue);
        }
        else
        {
            if (columnsCount >= _maxColumns)
            {
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _maxHeight);
            }
            else
            {
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _minHeight);
            }
        }
    }
}