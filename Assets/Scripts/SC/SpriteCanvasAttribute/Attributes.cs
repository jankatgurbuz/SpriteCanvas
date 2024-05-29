using UnityEngine;

namespace SC.SpriteCanvasAttribute
{
    public class SCSortingLayerAttribute : PropertyAttribute
    {
    }

    public class SCHorizontalLineAttribute : PropertyAttribute
    {
        public float Height { get; }
        public float Padding { get; }
        public EColor Color { get; }

        public SCHorizontalLineAttribute(EColor color, float height = 1f, float padding = 5f)
        {
            Height = height;
            Padding = padding;
            Color = color;
        }
    }
    public class CanvasKeyAttribute : PropertyAttribute
    {
    }
}