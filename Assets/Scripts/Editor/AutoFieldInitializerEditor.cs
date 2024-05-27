using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.ResponsiveOperations;
using Core.UI;
using Editor.Edit;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIElement), true)]
    public class AutoFieldInitializerEditor : UnityEditor.Editor
    {
        private readonly BindingFlags _bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private const string _responsiveOperationFieldName = "_responsiveOperation";
        private const string _itemPositionFieldName = "_itemPosition";
        private const string _spriteRendererFieldName = "_spriteRenderer";
        private Transform _itemPosition;

        private List<Type> _types;
        private List<IResponsiveOperation> implementingTypes;
        private string[] typeNames;
        private bool _initFlag;

        private void OnEnable()
        {
            _initFlag = false;
            _types = EditorHelpers.GetAssemblies();
            implementingTypes = EditorHelpers.GenerateTypeInstances<IResponsiveOperation>(_types);
            typeNames = EditorHelpers.ConvertTypeListToArray(implementingTypes);
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

            var currentIndex = EditorHelpers.FindTypeIndexInArray(implementingTypes, data);

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
            EditorHelpers.DrawPropertiesAutomatically(so);
            so.ApplyModifiedProperties();
        }
    }
}