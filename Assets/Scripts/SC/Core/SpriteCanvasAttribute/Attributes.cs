using SC.Core.UI;
using UnityEngine;

namespace SC.Core.SpriteCanvasAttribute
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

    public class CanvasKeyValidatorAttribute : PropertyAttribute
    {
    }

    public class SyncAlphaAttribute : PropertyAttribute
    {
        public bool RunTimeSync;

        public SyncAlphaAttribute(bool runTimeSync = false)
        {
            RunTimeSync = runTimeSync;
        }
    }
    public class ConditionalVisibilityAttribute : PropertyAttribute
    {
        public string EnumFieldName;
        public UIElement.RegisterType EnumValue;

        public ConditionalVisibilityAttribute(string enumFieldName, UIElement.RegisterType enumValue)
        {
            EnumFieldName = enumFieldName;
            EnumValue = enumValue;
        }
    }
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}