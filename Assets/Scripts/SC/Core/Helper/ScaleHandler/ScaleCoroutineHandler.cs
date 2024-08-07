using System.Collections;
using SC.Core.Helper.Groups;
using UnityEngine;
using UnityEngine.Events;

namespace SC.Core.Helper.ScaleHandler
{
    public class ScaleCoroutineHandler : IGroupSelectorHandler
    {
        private Coroutine _coroutine;

        public void AdjustItemsScale(GroupSelector groupSelector, float animationDuration,
            int currentSelectedIndex, float selectedItemScale, float unselectedItemScale, AnimationCurve scaleCurve,
            IGroup uiGroup, UnityEvent<int> onSelectionChanged, UnityEvent<int> onScaleUpdated,
            UnityEvent<int> onScaleAdjustmentComplete)
        {
            if (_coroutine != null)
            {
                groupSelector.StopCoroutine(_coroutine);
            }

            _coroutine = groupSelector.StartCoroutine(AdjustItemsScaleCoroutine(animationDuration, currentSelectedIndex,
                selectedItemScale, unselectedItemScale, scaleCurve, uiGroup,
                onSelectionChanged, onScaleUpdated, onScaleAdjustmentComplete));
        }

        private IEnumerator AdjustItemsScaleCoroutine(float animationDuration, int currentSelectedIndex,
            float selectedItemScale, float unselectedItemScale, AnimationCurve scaleCurve,
            IGroup uiGroup, UnityEvent<int> onSelectionChanged, UnityEvent<int> onScaleUpdated,
            UnityEvent<int> onScaleAdjustmentComplete)
        {
            var time = 0f;
            var useScaleCurve = scaleCurve != null && scaleCurve.keys.Length > 0;

            var initialScales = new float[uiGroup.GetUIElementList.Count];
            for (int index = 0; index < uiGroup.GetUIElementList.Count; index++)
            {
                initialScales[index] = uiGroup.GetUIElementList[index].ScaleRatio;
                if (index == currentSelectedIndex)
                {
                    onSelectionChanged?.Invoke(index);
                }
            }

            while (time < animationDuration)
            {
                for (var index = 0; index < uiGroup.GetUIElementList.Count; index++)
                {
                    var item = uiGroup.GetUIElementList[index];
                    var targetScale = index == currentSelectedIndex ? selectedItemScale : unselectedItemScale;
                    var initialScale = initialScales[index];
                    var t = time / animationDuration;

                    item.ScaleRatio = useScaleCurve
                        ? initialScale + (targetScale - initialScale) * scaleCurve.Evaluate(t)
                        : initialScale + (targetScale - initialScale) * t;
                }

                time += Time.deltaTime;
                onScaleUpdated?.Invoke(currentSelectedIndex);
                uiGroup.GetUIElement.Register.SpriteCanvas.AdjustDependentUIElements();
                yield return null;
            }

            for (var index = 0; index < uiGroup.GetUIElementList.Count; index++)
            {
                var item = uiGroup.GetUIElementList[index];
                item.ScaleRatio = index == currentSelectedIndex ? selectedItemScale : unselectedItemScale;
            }
            
            onScaleAdjustmentComplete?.Invoke(currentSelectedIndex);
            uiGroup.GetUIElement.Register.SpriteCanvas.AdjustDependentUIElements();
        }
    }
}