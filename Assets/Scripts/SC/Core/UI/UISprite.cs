using SC.Core.Helper;
using UnityEngine;

namespace SC.Core.UI
{
    public class UISprite : UIElement
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] private Vector3 _spriteSize;

        public override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            if (_spriteRenderer == null) return;
            _spriteRenderer.sortingLayerName = sortingLayer;
            _spriteRenderer.sortingOrder = Mathf.Max(sortingOrder, sortingOrder + _orderInLayer);
        }

        public override void SetUIElementProperties(SpriteCanvas.UIElementProperties elementProperties)
        {
            if (_spriteRenderer == null) return;
            var color = _spriteRenderer.color;
            color.a = Mathf.Min(elementProperties.Alpha, _alpha);
            _spriteRenderer.color = color;
        }

        public override void SetUILayout(float screenHeight, float screenWidth,
            Vector3 viewportCenterPosition, float balance, Vector3 groupAxisConstraint)
        {
            if (_spriteRenderer == null) return;
            _spriteRenderer.size = _spriteSize;

            Vector3 size = _spriteRenderer.drawMode == SpriteDrawMode.Simple
                ? _spriteRenderer.sprite.bounds.size // todo
                : _spriteRenderer.size;
            Handle(size, screenHeight, screenWidth, viewportCenterPosition, balance,groupAxisConstraint);
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