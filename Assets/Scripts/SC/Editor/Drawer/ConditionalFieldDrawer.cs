using SC.Core.SpriteCanvasAttribute;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Drawer
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var conditional = (ConditionalFieldAttribute)attribute;
            var enumProperty = property.serializedObject.FindProperty(conditional.EnumFieldName);

            if (enumProperty != null && enumProperty.enumValueIndex == (int)conditional.EnumValue)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var conditional = (ConditionalFieldAttribute)attribute;
            var enumProperty = property.serializedObject.FindProperty(conditional.EnumFieldName);

            if (enumProperty != null && enumProperty.enumValueIndex == (int)conditional.EnumValue)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}