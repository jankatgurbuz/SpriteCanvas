using SC.Core.Helper;
using SC.Core.Helper.Groups;
using SC.Core.Helper.UIElementHelper;
using SC.Core.Manager;
using SC.Core.ResponsiveOperations;
using SC.Core.SpriteCanvasAttribute;
using UnityEngine;

namespace SC.Core.UI
{
    public abstract class UIElement : MonoBehaviour
    {
        [SerializeReference] protected IResponsiveOperation _responsiveOperation;
        [SerializeField] private RegisterProperties _register;
        [SerializeField] private UIElementProperties _uIElementProperties;
        [SerializeField] protected Transform _itemPosition;
        [SerializeField] protected bool _hasReference;
        [SerializeField] protected UIElement _referenceElement;
        [SerializeField, SyncAlpha] protected float _alpha = 1;

        private bool _isChecked = true;
        private bool _isGroupChecked = true;
        private Vector3 _groupAxisConstraint = Vector3.one;
        
        public IResponsiveOperation ResponsiveOperation => _responsiveOperation;
        public UIElementProperties UIElementProperties => _uIElementProperties;
        public bool IsGroupChecked => _isGroupChecked;
        public RegisterProperties Register => _register;
        public UIElement ReferenceElement => _referenceElement;
        public Vector3 GroupAxisConstraint => _groupAxisConstraint;
        
        protected abstract void SetUILayout();
        protected abstract void ArrangeLayers(string sortingLayer, int sortingOrder);
        public abstract void SetUIElementProperties(UIElementSettings elementProperties);
        public abstract Vector3 GetBoundarySize();
        public abstract Vector3 GetElementSize();
        public abstract Vector3 GetRenderBoundarySize();
        public virtual SpriteDrawMode GetDrawMode() => SpriteDrawMode.Simple;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            GetSpriteCanvas();
            InitRegister();
        }

        private void GetSpriteCanvas()
        {
            switch (_register.RegisterType)
            {
                case RegisterType.Hierarchy:
                    _register.SpriteCanvas = FindParentSpriteCanvas(transform);

                    if (_register.SpriteCanvas == null)
                        Debug.LogWarningFormat("No SpriteCanvas found in parent hierarchy. " +
                                               "Object name: <color=orange>\"{0}\"</color>", gameObject.name);
                    break;

                case RegisterType.Reference:
                    if (_register.SpriteCanvas == null)
                        Debug.LogWarningFormat(
                            "SpriteCanvas reference is not assigned! Please drag and drop a reference. " +
                            "Object name: <color=orange>\"{0}\"</color>", gameObject.name);
                    break;

                case RegisterType.Key:
#if UNITY_EDITOR
                    var spriteCanvasArray = FindObjectsOfType<SpriteCanvas>();

                    foreach (var item in spriteCanvasArray)
                    {
                        if (item.CanvasKey != _register.CanvasKey) continue;
                        
                        _register.SpriteCanvas = item;
                    }
#endif
                    if (Application.isPlaying)
                    {
                        var register = SpriteCanvasManager.Instance.GetSpriteCanvas(_register.CanvasKey);
                        if (register == null)
                        {
                            Debug.LogError(
                                $"No SpriteCanvas found with key \" {_register.CanvasKey} \". Object = {name}");
                        }

                        _register.SpriteCanvas = register;
                    }

                    break;
            }
        }

        private void InitRegister()
        {
            if (_register.SpriteCanvas != null)
            {
                _register.SpriteCanvas.AddUI(this);
            }

            if (SpriteCanvasManager.Instance != null)
            {
                SpriteCanvasManager.Instance.RegisterTarget(_uIElementProperties.TargetKey, this);
            }
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
            if (!check) return group;

            var vec = group is HorizontalGroup ? new Vector3(0, 1) : new Vector3(1, 0);
            group.GetUIElementList.ForEach(x =>
            {
                if (x.UIElement != null)
                    x.UIElement._groupAxisConstraint = vec;
            });

            return group;
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

        public void Adjust()
        {
            if (!_isChecked && _hasReference)
            {
                _referenceElement.Adjust();
            }

            _ = CheckGroup();
            ArrangeLayers(_register.SpriteCanvas.SortingLayerName, _register.SpriteCanvas.SortingLayerOrder);
            SetUILayout();
            SetUIElementProperties(_register.SpriteCanvas.ElementProperties);
            AdjustGroup();

            _isChecked = true;
        }

        public void ResetFlags()
        {
            _isChecked = false;
            _isGroupChecked = false;
        }
    }
}