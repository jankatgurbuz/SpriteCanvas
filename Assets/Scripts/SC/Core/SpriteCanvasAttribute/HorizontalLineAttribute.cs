using SC.Core.Utility;
using UnityEngine;

namespace SC.Core.SpriteCanvasAttribute
{
    public class HorizontalLineAttribute : PropertyAttribute
    {
        public float Height { get; }
        public float Padding { get; }
        public EColor Color { get; }

        public HorizontalLineAttribute(EColor color, float height = 1f, float padding = 5f)
        {
            Height = height;
            Padding = padding;
            Color = color;
        }
    }
}