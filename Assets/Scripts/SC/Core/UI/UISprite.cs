using UnityEngine;

namespace SC.Core.UI
{
    public class UISprite : UIElement
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] private Vector3 _spriteSize;

        public override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            _spriteRenderer.sortingLayerName = sortingLayer;
            _spriteRenderer.sortingOrder = Mathf.Max(sortingOrder, sortingOrder + _orderInLayer);
        }

        public override void SetUILayout(float screenHeight, float screenWidth,
            Vector3 viewportCenterPosition, float balance)
        {
            _spriteRenderer.size = _spriteSize;
            Handle(_spriteRenderer.sprite.bounds.size, screenHeight, screenWidth, viewportCenterPosition, balance);
        }
    }
}