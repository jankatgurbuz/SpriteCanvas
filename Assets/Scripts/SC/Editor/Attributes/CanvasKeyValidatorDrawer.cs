using System.Reflection;
using SC.Core.SpriteCanvasAttribute;
using SC.Core.UI;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(CanvasKeyValidatorAttribute))]
    public class CanvasKeyValidatorDrawer : PropertyDrawer
    {
        private readonly BindingFlags _bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect textFieldRect = new Rect(position.x, position.y, position.width, position.height);
            property.stringValue = EditorGUI.TextField(textFieldRect, label, property.stringValue);

            if (!IsValidCanvasKey(property.stringValue))
            {
                Rect warningRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
                    position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.HelpBox(warningRect, "Invalid canvas key!", MessageType.Error);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight;
        }

        private bool IsValidCanvasKey(string key)
        {
            var sc = Resources.FindObjectsOfTypeAll<SpriteCanvas>();

            foreach (var spriteCanvas in sc)
            {
                var field = spriteCanvas.GetType().GetField("_canvasKey", _bindingFlags);
                if (field == null) continue;

                var value = field.GetValue(spriteCanvas);

                if (value is not string str) continue;

                if (str == key)
                    return true;
            }

            return false;
        }
    }
}