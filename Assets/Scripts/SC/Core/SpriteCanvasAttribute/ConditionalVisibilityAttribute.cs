using SC.Core.Helper.UIElementHelper;
using UnityEngine;

namespace SC.Core.SpriteCanvasAttribute
{
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
}