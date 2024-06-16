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
            Vector3 viewportCenterPosition, float balance)
        {
            if (_spriteRenderer == null) return;
            _spriteRenderer.size = _spriteSize;

            Vector3 size = _spriteRenderer.drawMode == SpriteDrawMode.Simple
                ? _spriteRenderer.sprite.bounds.size
                : _spriteRenderer.size;
            Handle(size, screenHeight, screenWidth, viewportCenterPosition, balance);
        }

        protected override SpriteDrawMode GetDrawMode()
        {
            return _spriteRenderer.drawMode;
        }

        protected override Vector3 GetBoundsSize()
        {
            return _spriteRenderer.sprite.bounds.size;
        }

        protected override Vector3 GetSize()
        {
            return _spriteRenderer.size;
        }
    }
}