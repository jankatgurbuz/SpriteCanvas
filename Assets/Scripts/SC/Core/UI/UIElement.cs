using SC.Core.ResponsiveOperations;
using SC.Manager;
using SC.SpriteCanvasAttribute;
using UnityEngine;

namespace SC.Core.UI
{
    public abstract class UIElement : MonoBehaviour
    {
        [SerializeReference] protected IResponsiveOperation _responsiveOperation;

        [SerializeField, SCHorizontalLine(EColor.Orange, 1, 0), CanvasKey]
        private string _canvasKey;

        [SerializeField] private string _targetKey;

        [SerializeField, SCHorizontalLine(EColor.Orange, 1, 3)]
        protected Transform _itemPosition;

        [SerializeField] protected int _orderInLayer;
        [SerializeField] protected SpriteRenderer _referenceSprite;

        [SerializeField] protected bool _hasReference;
        protected SpriteCanvas _spriteCanvas;
        public string CanvasKey => _canvasKey;

        public abstract void SetUILayout(float height, float width, Vector3 viewportCenterPosition, float balance);
        public abstract void ArrangeLayers(string sortingLayer, int sortingOrder);

        protected void Start()
        {
            _spriteCanvas = SpriteCanvasManager.Instance.Get(_canvasKey);
            SpriteCanvasManager.Instance.RegisterTarget(_targetKey, this);
            ArrangeLayers(_spriteCanvas.SortingLayerName, _spriteCanvas.SortingLayerOrder);
            SetUILayout(_spriteCanvas.ViewportHeight, _spriteCanvas.ViewportWidth,
                _spriteCanvas.ViewportPosition,
                _spriteCanvas.Balance);
        }

        protected void Handle(Vector3 boundsSize, float screenHeight, float screenWidth,
            Vector3 viewportCenterPosition, float balance)
        {
            if (!_hasReference)
            {
                _responsiveOperation.AdjustUI(screenHeight, screenWidth, boundsSize, _itemPosition,
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
                    boundsSize,
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