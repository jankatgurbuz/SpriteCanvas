using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class UITextMeshPro : UIElement
    {
        [SerializeField] private TextMeshPro _textMeshPro;
        [SerializeField] private Vector3 _textMeshProSize;

        public override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            _textMeshPro.sortingLayerID = SortingLayer.NameToID(sortingLayer);
            _textMeshPro.sortingOrder = Mathf.Max(sortingOrder, sortingOrder + _orderInLayer);
        }

        public override void SetUILayout(float screenHeight, float screenWidth,
            Vector3 viewportCenterPosition, float balance)
        {
            _textMeshPro.rectTransform.sizeDelta = _textMeshProSize;
            Handle(_textMeshPro.bounds.size, screenHeight, screenWidth, viewportCenterPosition, balance);
        }
    }
}