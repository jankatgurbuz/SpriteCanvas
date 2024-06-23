using System;
using SC.Core.Helper;
using SC.Core.Manager;
using SC.Core.ResponsiveOperations;
using SC.Core.SpriteCanvasAttribute;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] protected bool _hasReference;
        [SerializeField] protected UIElement _referenceElement;
        [SerializeField, SyncAlpha] protected float _alpha = 1;

        private bool _isChecked = true;
        private bool _isGroupChecked = true;
        public string CanvasKey => _canvasKey;
        public IResponsiveOperation ResponsiveOperation => _responsiveOperation;
        public Vector3 GroupAxisConstraint { get; private set; } = Vector3.one;

        public abstract void SetUILayout(float height, float width, Vector3 viewportCenterPosition, float balance,
            Vector3 groupAxisConstraint);

        public abstract void ArrangeLayers(string sortingLayer, int sortingOrder);
        public abstract void SetUIElementProperties(UIElementProperties elementProperties);
        public abstract Vector3 GetBoundarySize();
        public abstract Vector3 GetElementSize();
        public abstract Vector3 GetRenderBoundarySize();

        protected virtual SpriteDrawMode GetDrawMode()
        {
            return SpriteDrawMode.Simple;
        }

        private void Start()
        {
            Initialize();
        }

        public void InitialCheck()
        {
            _isChecked = false;
            _isGroupChecked = false;
        }

        public void Initialize()
        {
            GetSpriteCanvas();
            Register();
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
#if UNITY_EDITOR
                    var spriteCanvasArray = FindObjectsOfType<SpriteCanvas>();

                    foreach (var item in spriteCanvasArray)
                    {
                        if (item.CanvasKey == _canvasKey)
                        {
                            _spriteCanvas = item;
                        }
                    }
#endif
                    if (Application.isPlaying)
                    {
                        _spriteCanvas = SpriteCanvasManager.Instance.GetSpriteCanvas(_canvasKey);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Register()
        {
            _spriteCanvas.AddUI(this);
            if (SpriteCanvasManager.Instance == null)
            {
                return;
            }

            SpriteCanvasManager.Instance.RegisterTarget(_targetKey, this);
        }

        public void Adjust()
        {
            if (!_isChecked && _hasReference)
            {
                _referenceElement.Adjust();
            }

            _ = CheckGroup();
            ArrangeLayers(_spriteCanvas.SortingLayerName, _spriteCanvas.SortingLayerOrder);
            SetUILayout(
                _spriteCanvas.ViewportHeight,
                _spriteCanvas.ViewportWidth,
                _spriteCanvas.ViewportPosition,
                _spriteCanvas.Balance, GroupAxisConstraint);
            SetUIElementProperties(_spriteCanvas.ElementProperties);
            AdjustGroup();

            _isChecked = true;
        }

        private void AdjustGroup()
        {
            if (!_isGroupChecked && _hasReference)
            {
                _referenceElement.AdjustGroup();
            }

            var group = CheckGroup();
            if (group == null) return;

            group.AdjustGroup();
            _isGroupChecked = true;
        }

        private IGroup CheckGroup()
        {
            var check = TryGetComponent(out IGroup group);
            if (check)
            {
                var vec = group is HorizontalGroup ? new Vector3(0, 1) : new Vector3(1, 0);
                group.GetUIElementList.ForEach(x =>
                {
                    if (x.UIElement != null)
                        x.UIElement.GroupAxisConstraint = vec;
                });
            }

            return group;
        }

        protected void Handle(Vector3 boundsSize, float screenHeight, float screenWidth,
            Vector3 viewportCenterPosition, float balance, Vector3 groupAxisConstraint)
        {
            if (!_hasReference)
            {
                _responsiveOperation.AdjustUI(screenHeight, screenWidth, boundsSize, _itemPosition,
                    viewportCenterPosition, balance, groupAxisConstraint);
            }
            else
            {
                Vector3 referenceSpriteSize = _referenceElement.GetDrawMode() == SpriteDrawMode.Simple
                    ? _referenceElement.GetBoundarySize()
                    : _referenceElement.GetElementSize();

                var globalReferenceSize = GetGlobalSize(referenceSpriteSize, _referenceElement.transform);

                _responsiveOperation.AdjustUI(
                    globalReferenceSize.y,
                    globalReferenceSize.x,
                    boundsSize,
                    _itemPosition,
                    _referenceElement.transform.position, balance, groupAxisConstraint);
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