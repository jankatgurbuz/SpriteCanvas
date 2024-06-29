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

        public override void SetUIElementProperties(UIElementProperties elementProperties)
        {
            if (_spriteRenderer == null) return;
            var color = _spriteRenderer.color;
            color.a = Mathf.Min(elementProperties.Alpha, _alpha);
            _spriteRenderer.color = color;
        }

        public override void SetUILayout(float spriteCanvasViewportHeight, float spriteCanvasViewportWidth,
            Vector3 spriteCanvasViewportPosition, float spriteCanvasBalance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            if (_spriteRenderer == null) return;
            _spriteRenderer.size = _spriteSize;

            Vector3 size = _spriteRenderer.drawMode == SpriteDrawMode.Simple
                ? _spriteRenderer.sprite.bounds.size // todo !!
                : _spriteRenderer.size;
            Handle(size, spriteCanvasViewportHeight, spriteCanvasViewportWidth, spriteCanvasViewportPosition,
                spriteCanvasBalance, groupAxisConstraint, ignoreXPosition,
                ignoreYPosition, ignoreXScale, ignoreYScale);
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