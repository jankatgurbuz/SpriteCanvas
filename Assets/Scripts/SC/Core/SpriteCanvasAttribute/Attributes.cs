using System;
using SC.Core.Helper.UIElementHelper;
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
        public RegisterType EnumValue;

        public ConditionalVisibilityAttribute(string enumFieldName, RegisterType enumValue)
        {
            EnumFieldName = enumFieldName;
            EnumValue = enumValue;
        }
    }

    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string MethodName { get; private set; }
        public Type TargetType { get; private set; }

        public ButtonAttribute(string methodName, Type targetType)
        {
            MethodName = methodName;
            TargetType = targetType;
        }
    }
}