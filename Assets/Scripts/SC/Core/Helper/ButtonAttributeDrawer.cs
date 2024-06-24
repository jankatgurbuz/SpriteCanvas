using SC.Core.SpriteCanvasAttribute;
using UnityEditor;
using UnityEngine;

namespace SC.Core.Helper
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

            if (GUI.Button(buttonRect, "Debug"))
            {
                MonoBehaviour targetObject = property.serializedObject.targetObject as MonoBehaviour;
                System.Reflection.MethodInfo methodInfo = buttonAttr.TargetType.GetMethod(buttonAttr.MethodName);
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