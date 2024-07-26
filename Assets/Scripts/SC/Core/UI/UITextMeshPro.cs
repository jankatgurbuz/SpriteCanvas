using SC.Core.Helper;
using SC.Core.Helper.UIElementHelper;
using TMPro;
using UnityEngine;

namespace SC.Core.UI
{
    public class UITextMeshPro : UIElement
    {
        [SerializeField] private TextMeshPro _textMeshPro;
        [SerializeField] private Vector3 _textMeshProSize = new Vector3(20, 20, 0);
        private Vector3? _initPosition;

        protected override void SetUILayout()
        {
            if (_textMeshPro == null) return;

            _textMeshPro.rectTransform.sizeDelta = _textMeshProSize;
            var sc = Register.SpriteCanvas;
            var referencePosition = sc.ViewportPosition;
            var referenceSize = new Vector2(sc.ViewportWidth, sc.ViewportHeight);

            if (_hasReference)
            {
                var referenceSpriteSize = _referenceElement.GetDrawMode() == SpriteDrawMode.Simple
                    ? _referenceElement.GetBoundarySize()
                    : _referenceElement.GetElementSize();

                referenceSize = GetGlobalSize(referenceSpriteSize, _referenceElement.transform);
                referencePosition = _referenceElement.transform.position;
            }

            if (Application.isPlaying)
            {
                _initPosition ??= referencePosition;
            }
            else
            {
                _initPosition = referencePosition;
            }

            var responsiveProp = new ResponsiveUIProp()
            {
                UiItemTransform = _itemPosition,
                Height = referenceSize.y,
                Width = referenceSize.x,
                Balance = sc.Balance,
                ReferencePosition = UIElementProperties.UseInitialCameraPosition
                    ? (Vector3)_initPosition
                    : referencePosition,
                UiItemSize = _textMeshPro.rectTransform.sizeDelta,
                GroupAxisConstraint = GroupAxisConstraint,
                IgnoreXPosition = UIElementProperties.IgnoreXPosition,
                IgnoreYPosition = UIElementProperties.IgnoreYPosition,
                IgnoreXScale = UIElementProperties.IgnoreXScale,
                IgnoreYScale = UIElementProperties.IgnoreYScale,
                Camera = Register.SpriteCanvas.CameraMode
            };
            _responsiveOperation.AdjustUI(responsiveProp);
        }

        private Vector3 GetGlobalSize(Vector3 localSize, Transform myTransform)
        {
            var globalScale = myTransform.lossyScale;
            return new Vector3(localSize.x * globalScale.x, localSize.y * globalScale.y, localSize.z * globalScale.z);
        }

        protected override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            if (_textMeshPro == null) return;

            _textMeshPro.sortingLayerID = SortingLayer.NameToID(sortingLayer);
            _textMeshPro.sortingOrder = Mathf.Max(sortingOrder, sortingOrder + UIElementProperties.OrderInLayer);
        }

        public override void SetUIElementProperties(UIElementSettings elementProperties)
        {
            if (_textMeshPro == null) return;

            var color = _textMeshPro.color;
            color.a = Mathf.Min(elementProperties.Alpha, _alpha);
            _textMeshPro.color = color;
            _textMeshPro.renderer.enabled = _textMeshPro.color.a != 0;
        }

        public override Vector3 GetBoundarySize()
        {
            return _textMeshPro.rectTransform.sizeDelta;
        }

        public override Vector3 GetElementSize()
        {
            return _textMeshPro.rectTransform.sizeDelta;
        }

        public override Vector3 GetRenderBoundarySize()
        {
            return _textMeshPro.rectTransform.sizeDelta;
        }
    }
}