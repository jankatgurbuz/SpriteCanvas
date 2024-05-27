using UnityEngine;

namespace Core.UI
{
    public class UISprite : UIElement
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            _spriteRenderer.sortingLayerName = sortingLayer;
            _spriteRenderer.sortingOrder = Mathf.Max(sortingOrder, sortingOrder + _orderInLayer);
        }

        public override void Handle(float screenHeight, float screenWidth,
            Vector3 viewportCenterPosition, float balance)
        {
            var spriteSize = _spriteRenderer.sprite.bounds.size;
            if (_referenceSprite == null)
            {
                _responsiveOperation.AdjustUI(screenHeight, screenWidth, spriteSize, _itemPosition,
                    viewportCenterPosition, balance);
            }
            else
            {
                var size = _referenceSprite.sprite.bounds.size;

                _responsiveOperation.AdjustUI(
                    size.y * _referenceSprite.transform.localScale.y,
                    size.x * _referenceSprite.transform.localScale.x,
                    spriteSize, _itemPosition,
                    _referenceSprite.transform.position, balance);
            }
        }
    }
}