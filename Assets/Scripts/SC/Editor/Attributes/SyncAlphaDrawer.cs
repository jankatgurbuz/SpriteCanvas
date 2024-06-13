using SC.Core.SpriteCanvasAttribute;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(SyncAlphaAttribute))]
    public class SyncAlphaDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;

            var alphaAttribute = (SyncAlphaAttribute)attribute;
            if (!alphaAttribute.RunTimeSync)
            {
                if (Application.isPlaying)
                    return;
            }
            
            var target = property.serializedObject.targetObject as MonoBehaviour;
            if (target == null) return;

            if (target.TryGetComponent<SpriteRenderer>(out var sr))
            {
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    property.floatValue = sr.color.a;
                }
            }
            else if (target.TryGetComponent<TextMeshPro>(out var tmp))
            {
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    property.floatValue = tmp.color.a;
                }
            }
        }
    }
}