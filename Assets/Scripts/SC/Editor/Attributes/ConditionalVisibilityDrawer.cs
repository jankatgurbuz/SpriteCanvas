using SC.Core.SpriteCanvasAttribute;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(ConditionalVisibilityAttribute))]
    public class ConditionalVisibilityDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var conditional = (ConditionalVisibilityAttribute)attribute;
            var enumProperty = property.serializedObject.FindProperty(conditional.EnumFieldName);

            if (enumProperty != null && enumProperty.enumValueIndex == (int)conditional.EnumValue)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var conditional = (ConditionalVisibilityAttribute)attribute;
            var enumProperty = property.serializedObject.FindProperty(conditional.EnumFieldName);

            if (enumProperty != null && enumProperty.enumValueIndex == (int)conditional.EnumValue)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}