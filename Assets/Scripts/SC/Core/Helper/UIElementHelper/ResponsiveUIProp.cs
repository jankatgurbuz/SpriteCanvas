using UnityEngine;

namespace SC.Core.Helper.UIElementHelper
{
    public struct ResponsiveUIProp
    {
        public Transform UiItemTransform { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float Balance { get; set; }
        public Vector3 ReferencePosition { get; set; }
        public Vector3 UiItemSize { get; set; }
        public Vector3 GroupAxisConstraint { get; set; }
        public bool IgnoreXPosition { get; set; }
        public bool IgnoreYPosition { get; set; }
        public bool IgnoreXScale { get; set; }
        public bool IgnoreYScale { get; set; }
        public Camera Camera { get; set; }
    }
}