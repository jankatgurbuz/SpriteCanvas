using SC.Core.Helper;
using SC.Core.Helper.UIElementHelper;
using UnityEngine;

namespace SC.Core.UI
{
    public class UISprite : UIElement
    {
        [SerializeField, HideInInspector] protected SpriteRenderer _spriteRenderer;
        [SerializeField] private Vector3 _spriteSize = Vector3.one;

        protected override void SetUILayout()
        {
            if (_spriteRenderer == null) return;
            _spriteRenderer.size = _spriteSize;

            var size = _spriteRenderer.drawMode == SpriteDrawMode.Simple ? GetBoundarySize() : GetElementSize();
            var sc = Register.SpriteCanvas;
            var referencePosition = sc.ViewportPosition;
            var referenceSize = new Vector2(sc.ViewportWidth, sc.ViewportHeight);

            if (_hasReference)
            {
                var referenceSpriteSize = GetDrawMode() == SpriteDrawMode.Simple
                    ? _referenceElement.GetBoundarySize()
                    : _referenceElement.GetElementSize();

                referenceSize = GetGlobalSize(referenceSpriteSize, _referenceElement.transform);
                referencePosition = _referenceElement.transform.position;
            }
            
            var responsiveProp = new ResponsiveUIProp()
            {
                UiItemTransform = _itemPosition,
                Height = referenceSize.y,
                Width = referenceSize.x,
                Balance = sc.Balance,
                ReferencePosition = referencePosition,
                UiItemSize = size,
                GroupAxisConstraint = GroupAxisConstraint,
                IgnoreXPosition = UIElementProperties.IgnoreXPosition,
                IgnoreYPosition = UIElementProperties.IgnoreYPosition,
                IgnoreXScale = UIElementProperties.IgnoreXScale,
                IgnoreYScale = UIElementProperties.IgnoreYScale,
                Camera = Register.SpriteCanvas.Camera
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
            if (_spriteRenderer == null) return;
            _spriteRenderer.sortingLayerName = sortingLayer;
            _spriteRenderer.sortingOrder = Mathf.Max(sortingOrder, sortingOrder + UIElementProperties.OrderInLayer);
        }

        public override void SetUIElementProperties(UIElementSettings elementProperties)
        {
            if (_spriteRenderer == null) return;
            var color = _spriteRenderer.color;
            color.a = Mathf.Min(elementProperties.Alpha, _alpha);
            _spriteRenderer.color = color;
        }

        protected override SpriteDrawMode GetDrawMode()
        {
            return _spriteRenderer.drawMode;
        }

        public override Vector3 GetBoundarySize()
        {
            return _spriteRenderer.sprite.bounds.size;
        }

        public override Vector3 GetElementSize()
        {
            return _spriteRenderer.size;
        }

        public override Vector3 GetRenderBoundarySize()
        {
            return _spriteRenderer.bounds.size;
        }
    }
}