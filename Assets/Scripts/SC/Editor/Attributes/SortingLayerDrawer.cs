using SC.Core.SpriteCanvasAttribute;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(SCSortingLayerAttribute))]
    public class SortingLayerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                var sortingLayerNames = GetSortingLayerNames();
                var currentIndex = System.Array.IndexOf(sortingLayerNames, property.stringValue);

                if (currentIndex == -1)
                    currentIndex = 0;

                currentIndex = EditorGUI.Popup(position, label.text, currentIndex, sortingLayerNames);
                property.stringValue = sortingLayerNames[currentIndex];
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use SortingLayer with string.");
            }
        }

        private string[] GetSortingLayerNames()
        {
            var sortingLayerCount = SortingLayer.layers.Length;
            var sortingLayerNames = new string[sortingLayerCount];
            for (int i = 0; i < sortingLayerCount; i++)
            {
                sortingLayerNames[i] = SortingLayer.layers[i].name;
            }
            return sortingLayerNames;
        }
    }
}