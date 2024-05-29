using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.ResponsiveOperations;
using Core.UI;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIElement), true)]
    public class UIElementEditor : UnityEditor.Editor
    {
        private readonly BindingFlags _bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private const string _responsiveOperationFieldName = "_responsiveOperation";
        private const string _itemPositionFieldName = "_itemPosition";
        private const string _spriteRendererFieldName = "_spriteRenderer";
        private const string _textMeshProFieldName = "_textMeshPro";

        private Transform _itemPosition;

        private List<IResponsiveOperation> implementingTypes;
        private string[] typeNames;
        private bool _initFlag;

        private void OnEnable()
        {
            _initFlag = false;
            implementingTypes = GenerateTypeInstances<IResponsiveOperation>();
            typeNames = ConvertTypeListToArray(implementingTypes);
        }

        public override void OnInspectorGUI()
        {
            AssignComponent();
            FillResponsiveField();
            DrawPropertiesAutomatically();
        }

        private void FillResponsiveField()
        {
            var script = (UIElement)target;
            var data = GetValue<ResponsiveOperation>(script, _responsiveOperationFieldName, out _);

            var currentIndex = FindTypeIndexInArray(implementingTypes, data);

            EditorGUI.BeginChangeCheck();
            var selectedIndex = EditorGUILayout.Popup("Responsive", currentIndex, typeNames);
            if (!_initFlag || EditorGUI.EndChangeCheck())
            {
                ReflectSpriteOperation(script, _responsiveOperationFieldName, implementingTypes[selectedIndex]);
                EditorUtility.SetDirty(script);
                _initFlag = true;
            }
        }

        private void ReflectSpriteOperation(UIElement sprite, string targetFieldName,
            IResponsiveOperation spriteUIOperationHandler)
        {
            const BindingFlags instanceNonPublicFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            var spriteFields = sprite.GetType().GetFields(instanceNonPublicFlags).ToList();
            var targetField = spriteFields.Find(field => field.Name == targetFieldName);

            // todo: Bad method. It needs fixing.
            if (sprite is UISprite)
            {
                object spriteObject = sprite switch
                {
                    UISprite => sprite,

                    _ => null
                };

                if (targetField.GetValue(spriteObject) == null)
                    targetField.SetValue(spriteObject, spriteUIOperationHandler);

                if (targetField.GetValue(spriteObject)?.GetType() == spriteUIOperationHandler.GetType())
                    return;
            }

            targetField.SetValue(sprite, spriteUIOperationHandler);
        }

        private void AssignComponent()
        {
            var element = (UIElement)target;
            AssignComponent<Transform>(element, _itemPositionFieldName);
            AssignComponent<SpriteRenderer>(element, _spriteRendererFieldName);
            AssignComponent<TextMeshPro>(element, _textMeshProFieldName);
        }

        private void AssignComponent<T>(UIElement uiElement, string fieldName) where T : Component
        {
            var component = GetValue<T>(uiElement, fieldName, out FieldInfo field);
            if (component != null) return;

            component = uiElement.GetComponent<T>();
            if (component != null)
            {
                field.SetValue(uiElement, component);

                EditorUtility.SetDirty(uiElement);
                PrefabUtility.RecordPrefabInstancePropertyModifications(uiElement);
                AssetDatabase.SaveAssets();
            }
        }

        private FieldInfo GetFieldInfo(object obj, string fieldName)
        {
            return obj.GetType().GetField(fieldName, _bindingFlags);
        }

        private T GetValue<T>(object obj, string fieldName, out FieldInfo fieldInfo)
        {
            var field = GetFieldInfo(obj, fieldName);

            if (field != null)
            {
                fieldInfo = field;
                return (T)field.GetValue(obj);
            }

            fieldInfo = null;
            return default;
        }

        private void DrawPropertiesAutomatically()
        {
            var script = (UIElement)target;
            var so = new SerializedObject(script);
            DrawPropertiesAutomatically(so);
            so.ApplyModifiedProperties();
        }
        
        // helpers

        private static IEnumerable<Type> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()).ToList();
        }

        private static List<T> GenerateTypeInstances<T>()
        {
            var typeList = GetAssemblies();
            return typeList.Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Select(type => (T)Activator.CreateInstance(type))
                .ToList();
        }

        private static string[] ConvertTypeListToArray<T>(List<T> list)
        {
            return list.Select(type => type.GetType().ToString().Split('.').Last()).ToArray();
        }

        private static int FindTypeIndexInArray<T>(List<T> array, T field)
        {
            if (field == null)
                return 0;

            return array.FindIndex(type => type.GetType() == field.GetType());
        }

        private static void DrawPropertiesAutomatically(SerializedObject so)
        {
            var serializedProperty = so.GetIterator();
            serializedProperty.NextVisible(true);
            while (serializedProperty.NextVisible(false))
            {
                EditorGUILayout.PropertyField(serializedProperty, true);
            }
        }
    }
}