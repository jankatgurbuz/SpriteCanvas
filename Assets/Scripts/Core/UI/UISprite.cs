using UnityEngine;

namespace Core.UI
{
    public class UISprite : UIElement
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Vector3 _spriteSize;

        public override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            _spriteRenderer.sortingLayerName = sortingLayer;
            _spriteRenderer.sortingOrder = Mathf.Max(sortingOrder, sortingOrder + _orderInLayer);
        }

        public override void Handle(float screenHeight, float screenWidth,
            Vector3 viewportCenterPosition, float balance)
        {
            var spriteSize = _spriteRenderer.sprite.bounds.size;
            _spriteRenderer.size = _spriteSize;
            
            if (_referenceSprite == null)
            {
                _responsiveOperation.AdjustUI(screenHeight, screenWidth, spriteSize, _itemPosition,
                    viewportCenterPosition, balance);
            }
            else
            {
                var referenceSpriteSize = _referenceSprite.sprite.bounds.size;
                var referenceTransform = _referenceSprite.transform;
                var globalReferenceSize = GetGlobalSize(referenceSpriteSize, referenceTransform);

                _responsiveOperation.AdjustUI(
                    globalReferenceSize.y,
                    globalReferenceSize.x,
                    spriteSize,
                    _itemPosition,
                    _referenceSprite.transform.position, balance);
            }
        }

        private Vector3 GetGlobalSize(Vector3 localSize, Transform myTransform)
        {
            var globalScale = myTransform.lossyScale;
            return new Vector3(localSize.x * globalScale.x, localSize.y * globalScale.y, localSize.z * globalScale.z);
        }
    }
}