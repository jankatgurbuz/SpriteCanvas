using SC.Core.Helper.UIElementHelper;
using UnityEngine;

namespace SC.Core.ResponsiveOperations
{
    public interface IResponsiveOperation
    {
        public void AdjustUI(ResponsiveUIProp prop);
        public Vector3 GetLocalScale();
    }
}