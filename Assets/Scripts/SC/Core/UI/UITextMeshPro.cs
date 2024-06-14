using TMPro;
using UnityEngine;

namespace SC.Core.UI
{
    public class UITextMeshPro : UIElement
    {
        [SerializeField] private TextMeshPro _textMeshPro;
        [SerializeField] private Vector3 _textMeshProSize = new Vector3(20, 20, 0);
        [SerializeField] private bool _ignoreSize;

        public override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            if (_textMeshPro == null) return;
            _textMeshPro.sortingLayerID = SortingLayer.NameToID(sortingLayer);
            _textMeshPro.sortingOrder = Mathf.Max(sortingOrder, sortingOrder + _orderInLayer);
        }

        public override void SetUIElementProperties(SpriteCanvas.UIElementProperties elementProperties)
        {
            if (_textMeshPro == null) return;
            var color = _textMeshPro.color;
            color.a = Mathf.Min(elementProperties.Alpha, _alpha);
            _textMeshPro.color = color;
        }

        public override void SetUILayout(float screenHeight, float screenWidth, Vector3 viewportCenterPosition,
            float balance)
        {
            if (_textMeshPro == null) return;
            _textMeshPro.rectTransform.sizeDelta = _textMeshProSize;
            Handle(_textMeshPro.rectTransform.sizeDelta, screenHeight, screenWidth, viewportCenterPosition, balance);
        }
    }
}