using System.Collections;
using System.Collections.Generic;
using SC.Core.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SC.Core.Helper.ScaleHandler
{
    public class ScaleCoroutineHandler : IGroupSelectorHandler
    {
        private Coroutine _coroutine;

        public void AdjustItemsScale(GroupSelector groupSelector, float animationDuration,
            int currentSelectedIndex, float selectedItemScale, float unselectedItemScale,
            AnimationCurve scaleCurve, IGroup uiGroup, UnityEvent<int> onSelectionChanged)
        {
            if (_coroutine != null)
            {
                groupSelector.StopCoroutine(_coroutine);
            }

            _coroutine = groupSelector.StartCoroutine(AdjustItemsScaleCoroutine(animationDuration, currentSelectedIndex,
                selectedItemScale, unselectedItemScale, scaleCurve, uiGroup, onSelectionChanged));
        }

        private IEnumerator AdjustItemsScaleCoroutine(float animationDuration, int currentSelectedIndex,
            float selectedItemScale,
            float unselectedItemScale, AnimationCurve scaleCurve, IGroup uiGroup, UnityEvent<int> onSelectionChanged)
        {
            var time = 0f;
            var useScaleCurve = scaleCurve != null && scaleCurve.keys.Length > 0;

            var initialScales = new float[uiGroup.GetUIElementList.Count];
            for (int index = 0; index < uiGroup.GetUIElementList.Count; index++)
            {
                initialScales[index] = uiGroup.GetUIElementList[index].ScaleRatio;
            }

            while (time < animationDuration)
            {
                for (var index = 0; index < uiGroup.GetUIElementList.Count; index++)
                {
                    var item = uiGroup.GetUIElementList[index];
                    float targetScale;

                    if (index == currentSelectedIndex)
                    {
                        targetScale = selectedItemScale;
                        onSelectionChanged?.Invoke(index);
                    }
                    else
                    {
                        targetScale = unselectedItemScale;
                    }

                    var initialScale = initialScales[index];
                    var t = time / animationDuration;
                    item.ScaleRatio = useScaleCurve
                        ? initialScale + (targetScale - initialScale) * scaleCurve.Evaluate(t)
                        : initialScale + (targetScale - initialScale) * t;
                }

                time += Time.deltaTime;
                uiGroup.GetUIElement.SpriteCanvas.AdjustDependentUIElements();
                yield return null;
            }
            
            for (var index = 0; index < uiGroup.GetUIElementList.Count; index++)
            {
                var item = uiGroup.GetUIElementList[index];
                item.ScaleRatio = index == currentSelectedIndex ? selectedItemScale : unselectedItemScale;
            }
            uiGroup.GetUIElement.SpriteCanvas.AdjustDependentUIElements();
        }
    }
}