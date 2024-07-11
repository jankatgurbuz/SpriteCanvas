using System.Collections.Generic;
using System.Linq;
using SC.Core.ResponsiveOperations;
using SC.Core.UI;
using UnityEngine;

namespace SC.Core.Helper.Groups
{
    public class HorizontalGroup : MonoBehaviour, IHorizontalGroup
    {
        [SerializeField] private List<GroupElementProperty> _childUIElementList;
        [SerializeField] private float _space;
        private UIElement _uiElement;
        public List<GroupElementProperty> GetUIElementList => _childUIElementList;
        public UIElement GetUIElement => _uiElement;

        private bool Initialize()
        {
            if (_uiElement == null)
            {
                if (!TryGetComponent(out _uiElement))
                {
                    Debug.LogWarning("UIElement component not found.");
                    return false;
                }
            }

            if (GetUIElementList.Any(item => item.UIElement == null))
            {
                Debug.LogWarning("UIElement in the list is null.");
                return false;
            }

            if (_uiElement.ResponsiveOperation is not (BottomStretch
                or TopStretch))
            {
                Debug.LogWarning("ResponsiveOperation is neither BottomStretch nor TopStretch.");
                return false;
            }

            return true;
        }

        public void AdjustGroup()
        {
            if (!Initialize()) return;

            var totalWidth = _uiElement.transform.localScale.x * _uiElement.GetBoundarySize().x;
            var totalScaleRatio = _childUIElementList.Sum(x => x.ScaleRatio);
            var currentPosition = -totalWidth / 2;

            foreach (var childElement in _childUIElementList)
            {
                var scaleFactor = childElement.ScaleRatio * (_childUIElementList.Count / totalScaleRatio);
                var scaledWidth = totalWidth / _childUIElementList.Count * scaleFactor;

                AdjustScale(childElement, scaledWidth);
                currentPosition = AdjustPosition(childElement, currentPosition, scaledWidth);
            }
        }

        private void AdjustScale(GroupElementProperty childElement, float scaledWidth)
        {
            var child = childElement.UIElement;
            var newScale = new Vector3
            (
                scaledWidth / child.GetBoundarySize().x - _space - childElement.SpaceRatio,
                child.transform.lossyScale.y,
                childElement.UIElement.ResponsiveOperation.GetLocalScale().z
            );
            newScale = GetLocalScaleIgnoringParent(child.transform, newScale);
            child.transform.localScale = newScale;
        }

        private float AdjustPosition(GroupElementProperty childElement, float currentPosition,
            float scaledWidth)
        {
            var child = childElement.UIElement;
            var localPosition = new Vector3(currentPosition + scaledWidth / 2, 0, 0);

            var cameraTransform = _uiElement.Register.SpriteCanvas.Camera.transform;
            var worldPosition = _uiElement.transform.position + cameraTransform.rotation * localPosition;
            var originalLocalPosition = child.transform.localPosition;

            child.transform.position = worldPosition;
            child.transform.localPosition = new Vector3(child.transform.localPosition.x,
                originalLocalPosition.y, originalLocalPosition.z);

            return currentPosition + scaledWidth;
        }

        private Vector3 GetLocalScaleIgnoringParent(Transform uiItemTransform, Vector3 scale)
        {
            if (uiItemTransform.parent == null) return scale;

            var parentScale = uiItemTransform.parent.lossyScale;
            scale = new Vector3(
                scale.x / parentScale.x,
                scale.y / parentScale.y,
                scale.z / parentScale.z
            );

            return scale;
        }
    }
}