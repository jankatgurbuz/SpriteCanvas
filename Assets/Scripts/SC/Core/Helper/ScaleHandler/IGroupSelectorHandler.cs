using SC.Core.Helper.Groups;
using UnityEngine;
using UnityEngine.Events;

namespace SC.Core.Helper.ScaleHandler
{
    public interface IGroupSelectorHandler
    {
        void AdjustItemsScale(GroupSelector groupSelector, float animationDuration, int currentSelectedIndex,
            float selectedItemScale, float unselectedItemScale, AnimationCurve scaleCurve, IGroup uiGroup,
            UnityEvent<int> onSelectionChanged, UnityEvent<int> onScaleUpdated,UnityEvent<int> onScaleAdjustmentComplete);
    }
}