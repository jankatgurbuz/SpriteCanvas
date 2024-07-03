using UnityEngine;

namespace SC.Core.Helper.UIElementHelper
{
    public struct ResponsiveUIProp
    {
        public Transform UiItemTransform;
        public float Height;
        public float Width;
        public float Balance;
        public Vector3 ReferencePosition;
        public Vector3 UiItemSize;
        public Vector3 GroupAxisConstraint;
        public bool IgnoreXPosition;
        public bool IgnoreYPosition;
        public bool IgnoreXScale;
        public bool IgnoreYScale;
    }
}