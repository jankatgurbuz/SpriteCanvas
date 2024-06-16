using System;
using SC.Core.Manager;
using SC.Core.ResponsiveOperations;
using SC.Core.SpriteCanvasAttribute;
using UnityEngine;

namespace SC.Core.UI
{
    public abstract class UIElement : MonoBehaviour
    {
        [SerializeReference] protected IResponsiveOperation _responsiveOperation;

        [SerializeField, SCHorizontalLine(EColor.Orange, 1, 0)]
        private RegisterType _registerType;

        [SerializeField, ConditionalVisibility("_registerType", RegisterType.Key), CanvasKeyValidator]
        private string _canvasKey;

        [SerializeField, ConditionalVisibility("_registerType", RegisterType.Reference)]
        protected SpriteCanvas _spriteCanvas;

        [SerializeField, SCHorizontalLine(EColor.Orange, 1, 3)]
        private string _targetKey;

        [SerializeField] protected Transform _itemPosition;

        [SerializeField] protected int _orderInLayer;
        [SerializeField] protected SpriteRenderer _referenceSprite;
        [SerializeField] protected bool _hasReference;
        [SerializeField, SyncAlpha] protected float _alpha = 1;

        public string CanvasKey => _canvasKey;

        public abstract void SetUILayout(float height, float width, Vector3 viewportCenterPosition, float balance);
        public abstract void ArrangeLayers(string sortingLayer, int sortingOrder);
        public abstract void SetUIElementProperties(SpriteCanvas.UIElementProperties elementProperties);

        protected void Start()
        {
            GetSpriteCanvas();
            Register();
            Adjust();
        }

        private void GetSpriteCanvas()
        {
            switch (_registerType)
            {
                case RegisterType.Hierarchy:
                    _spriteCanvas = FindParentSpriteCanvas(transform);

                    if (_spriteCanvas == null)
                    {
                        Debug.LogError("No SpriteCanvas found in parent hierarchy.");
                    }

                    break;

                case RegisterType.Reference:
                    if (_spriteCanvas == null)
                    {
                        Debug.LogError("SpriteCanvas reference is not assigned! Please drag and drop a reference.");
                    }

                    break;

                case RegisterType.Key:
                    _spriteCanvas = SpriteCanvasManager.Instance.GetSpriteCanvas(_canvasKey);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Register()
        {
            _spriteCanvas.AddUI(this);
            SpriteCanvasManager.Instance.RegisterTarget(_targetKey, this);
        }

        public void Adjust()
        {
            ArrangeLayers(_spriteCanvas.SortingLayerName, _spriteCanvas.SortingLayerOrder);
            SetUILayout(_spriteCanvas.ViewportHeight, _spriteCanvas.ViewportWidth,
                _spriteCanvas.ViewportPosition,
                _spriteCanvas.Balance);
            SetUIElementProperties(_spriteCanvas.ElementProperties);
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
                Vector3 referenceSpriteSize = _referenceSprite.drawMode == SpriteDrawMode.Simple
                    ? _referenceSprite.sprite.bounds.size
                    : _referenceSprite.size;

                var globalReferenceSize = GetGlobalSize(referenceSpriteSize, _referenceSprite.transform);

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

        private SpriteCanvas FindParentSpriteCanvas(Transform child)
        {
            var currentParent = child.parent;

            while (currentParent != null)
            {
                if (currentParent.TryGetComponent(out SpriteCanvas spriteCanvas))
                    return spriteCanvas;

                currentParent = currentParent.parent;
            }

            return null;
        }

        public enum RegisterType
        {
            Hierarchy,
            Reference,
            Key
        }
    }
}