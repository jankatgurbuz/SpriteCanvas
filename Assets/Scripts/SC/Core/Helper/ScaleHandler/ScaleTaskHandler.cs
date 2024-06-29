#if SPRITECANVAS_UNITASK_SUPPORT
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace SC.Core.Helper.ScaleHandler
{
    public class ScaleTaskHandler : IGroupSelectorHandler
    {
        private CancellationTokenSource _cancellationTokenSource;

        public void AdjustItemsScale(GroupSelector groupSelector, float animationDuration,
            int currentSelectedIndex, float selectedItemScale, float unselectedItemScale,
            AnimationCurve scaleCurve, IGroup uiGroup, UnityEvent<int> onSelectionChanged)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            AdjustItemsScaleTask(animationDuration, currentSelectedIndex,
                selectedItemScale, unselectedItemScale, scaleCurve, uiGroup, onSelectionChanged,
                _cancellationTokenSource.Token).Forget();
        }

        private async UniTask AdjustItemsScaleTask(float animationDuration, int currentSelectedIndex,
            float selectedItemScale, float unselectedItemScale, AnimationCurve scaleCurve,
            IGroup uiGroup, UnityEvent<int> onSelectionChanged, CancellationToken cancellationToken)
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
                if (cancellationToken.IsCancellationRequested) return;
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
                uiGroup.GetUIElement.SpriteCanvas.AdjustDependentUIElements();
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
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

#endif