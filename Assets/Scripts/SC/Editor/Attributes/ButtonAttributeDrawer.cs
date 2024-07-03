using SC.Core.SpriteCanvasAttribute;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var buttonAttr = attribute as ButtonAttribute;
            EditorGUI.BeginProperty(position, label, property);

            var buttonRect = new Rect(position.x, position.y, position.width / 2, position.height);
            var floatRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);

            var intField = EditorGUI.IntField(floatRect, label, property.intValue);

            var name = buttonAttr.ButtonName;
            if (buttonAttr.ButtonName == null)
            {
                name = buttonAttr.MethodName;
            }

            if (GUI.Button(buttonRect, name))
            {
                var targetObject = property.serializedObject.targetObject as MonoBehaviour;
                var methodInfo = buttonAttr.TargetType.GetMethod(buttonAttr.MethodName);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(targetObject, new object[] { intField });
                }
                else
                {
                    Debug.LogError("Method not found: " + buttonAttr.MethodName);
                }
            }

            property.intValue = intField;

            EditorGUI.EndProperty();
        }
    }
}