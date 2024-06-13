using SC.Core.SpriteCanvasAttribute;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(SCHorizontalLineAttribute))]
    public class HorizontalLineDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var lineAttribute = (SCHorizontalLineAttribute)attribute;

            position.y += lineAttribute.Padding / 2;
            position.height = lineAttribute.Height;

            EditorGUI.DrawRect(position, lineAttribute.Color.GetColor());
        }

        public override float GetHeight()
        {
            var lineAttribute = (SCHorizontalLineAttribute)attribute;
            return base.GetHeight() + lineAttribute.Height + lineAttribute.Padding;
        }
    }
}