using System.Collections.Generic;
using System.Linq;
using SC.Core.UI;
using UnityEngine;

namespace SC.Core.Helper
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

            if (_uiElement.ResponsiveOperation is not (ResponsiveOperations.BottomStretch
                or ResponsiveOperations.TopStretch)) // todo why ! center
            {
                Debug.LogWarning("ResponsiveOperation is neither BottomStretch nor TopStretch.");
                return false;
            }

            return true;
        }

        public void AdjustGroup()
        {
            if (!Initialize()) return;

            var totalWidth = _uiElement.GetRenderBoundarySize().x;
            var totalScaleRatio = _childUIElementList.Sum(x => x.ScaleRatio);
            //var initialPosition = -totalWidth / (2 * transform.lossyScale.x);
            var accumulatedPosition = 0f;

            foreach (var childElement in _childUIElementList)
            {
                var child = childElement.UIElement;
                var scaleFactor = childElement.ScaleRatio * (_childUIElementList.Count / totalScaleRatio);
                var scaledWidth = totalWidth / _childUIElementList.Count * scaleFactor;

                var scaleVector = new Vector3
                (
                    scaledWidth / child.GetBoundarySize().x - _space - childElement.SpaceRatio,
                    child.transform.lossyScale.y,
                    child.transform.lossyScale.z
                );

                if (child.transform.parent != null)
                {
                    scaleVector = child.transform.parent.InverseTransformVector(scaleVector);
                }

                child.transform.localScale = scaleVector;

                child.transform.position = new Vector3
                (
                    ((-totalWidth) / 2) + accumulatedPosition + scaledWidth / 2,
                    child.transform.position.y,
                    child.transform.position.z
                );

                accumulatedPosition += scaledWidth;
            }
        }
    }
}