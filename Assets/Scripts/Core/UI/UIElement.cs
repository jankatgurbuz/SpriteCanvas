using Core.ResponsiveOperations;
using Manager;
using SpriteCanvasAttribute;
using UnityEngine;

namespace Core.UI
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
        public string CanvasKey => _canvasKey;

        private void Start()
        {
            var spriteCanvas = SpriteCanvasManager.Instance.Get(_canvasKey);
            SpriteCanvasManager.Instance.RegisterTarget(_targetKey, this);
            ArrangeLayers(spriteCanvas.SortingLayerName, spriteCanvas.SortingLayerOrder);
            Handle(spriteCanvas.ViewportHeight, spriteCanvas.ViewportWidth, spriteCanvas.ViewportPosition,
                spriteCanvas.Balance);
        }

        public abstract void Handle(float height, float width, Vector3 viewportCenterPosition, float balance);
        public abstract void ArrangeLayers(string sortingLayer, int sortingOrder);
    }
}