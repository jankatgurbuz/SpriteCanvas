using SC.Core.SpriteCanvasAttribute;
using SC.Core.Utility;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
    public class HorizontalLineDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var lineAttribute = (HorizontalLineAttribute)attribute;

            position.y += lineAttribute.Padding / 2;
            position.height = lineAttribute.Height;

            EditorGUI.DrawRect(position, lineAttribute.Color.GetColor());
        }

        public override float GetHeight()
        {
            var lineAttribute = (HorizontalLineAttribute)attribute;
            return base.GetHeight() + lineAttribute.Height + lineAttribute.Padding;
        }
    }
}