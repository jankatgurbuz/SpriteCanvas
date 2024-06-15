using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SC.Core.ResponsiveOperations;
using SC.Core.UI;
using SC.Editor.Helpers;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Utilities
{
    [CustomEditor(typeof(UIElement), true)]
    public class UIElementEditor : UnityEditor.Editor
    {
        private const string ResponsiveOperationFieldName = "_responsiveOperation";
        private const string ItemPositionFieldName = "_itemPosition";
        private const string SpriteRendererFieldName = "_spriteRenderer";
        private const string TextMeshProFieldName = "_textMeshPro";
        private const string RegisterTypeFieldName = "_registerType";
        private const string CanvasKey = "_canvasKey";
        private const string SpriteCanvasFieldName = "_spriteCanvas";
        private const int ItemsPerPage = 5;

        private Transform _itemPosition;
        private List<IResponsiveOperation> _implementingTypes;
        private List<string> _keyList;
        private string[] _typeNames;
        private int _currentPage;
        private bool _initFlag;

        private void OnEnable()
        {
            _initFlag = false;
            _implementingTypes = GenerateTypeInstances<IResponsiveOperation>();
            _typeNames = ConvertTypeListToArray(_implementingTypes);
            EditorApplication.update += SpriteCanvasUpdater.OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= SpriteCanvasUpdater.OnEditorUpdate;
        }

        public override void OnInspectorGUI()
        {
            AssignComponent();
            FillResponsiveField();

            _keyList = CreateCanvasKeyList();

            var prop = serializedObject.GetIterator();
            var enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                EditorGUILayout.PropertyField(prop, true);
                if (prop.name != SpriteCanvasFieldName) continue;

                var value = (UIElement.RegisterType)GetValue<UIElement>(target, RegisterTypeFieldName);
                if (value != UIElement.RegisterType.Key) continue;
                ListElements();
                PageNavigation();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AssignComponent()
        {
            AssignComponent<Transform>(ItemPositionFieldName);
            AssignComponent<SpriteRenderer>(SpriteRendererFieldName);
            AssignComponent<TextMeshPro>(TextMeshProFieldName);

            void AssignComponent<T>(string fieldName) where T : Component
            {
                var currentValue = GetValue<T>(target, fieldName);
                var component = ((UIElement)target).GetComponent<T>();
            
                // if (component != null && currentValue == null)
                // {
                    SetValue<UIElement>(target, fieldName, component);
                // }
                // todo: check
            }
        }

        private void FillResponsiveField()
        {
            var data = GetValue<UIElement>(target, ResponsiveOperationFieldName);
            var currentIndex = FindTypeIndexInArray(_implementingTypes, (ResponsiveOperation)data);

            EditorGUI.BeginChangeCheck();
            var selectedIndex = EditorGUILayout.Popup("Anchor Presets", currentIndex, _typeNames);
            if (!_initFlag || EditorGUI.EndChangeCheck())
            {
                var spriteUIOperationHandler = _implementingTypes[selectedIndex];

                if (data == null || data.GetType() != spriteUIOperationHandler.GetType())
                {
                    SetValue<UIElement>(target, ResponsiveOperationFieldName, spriteUIOperationHandler);
                }

                EditorUtility.SetDirty(target);
                _initFlag = true;
            }
        }

        private List<string> CreateCanvasKeyList()
        {
            var list = new List<string>();
            var sc = Resources.FindObjectsOfTypeAll<SpriteCanvas>();
            foreach (var spriteCanvas in sc)
            {
                var value = GetValue<SpriteCanvas>(spriteCanvas, CanvasKey);

                if (value is not string str) continue;
                if (str == "") continue;

                list.Add(str);
            }

            return list;
        }

        private void ListElements()
        {
            var uICanvas = (UIElement)target;

            if (_keyList is not { Count: > 0 }) return;

            var startItem = _currentPage * ItemsPerPage;
            var endItem = Mathf.Min(startItem + ItemsPerPage, _keyList.Count);

            for (int i = startItem; i < endItem; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(_keyList[i], GUILayout.Width(150), GUILayout.Height(25)))
                {
                    SetValue<UIElement>(target, CanvasKey, _keyList[i]);
                    EditorUtility.SetDirty(uICanvas);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void PageNavigation()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<", GUILayout.Width(50)))
            {
                _currentPage = Mathf.Max(_currentPage - 1, 0);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(
                $"Page {_currentPage + 1}/{Mathf.CeilToInt((float)_keyList.Count / ItemsPerPage)}",
                GUILayout.Width(100));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(">", GUILayout.Width(50)))
            {
                _currentPage = Mathf.Min(_currentPage + 1,
                    Mathf.CeilToInt((float)_keyList.Count / ItemsPerPage) - 1);
            }

            EditorGUILayout.EndHorizontal();
        }

        // helpers
        private FieldInfo GetField<T>(string fieldName, Type type)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance |
                                       BindingFlags.FlattenHierarchy;
            var field = typeof(T).GetField(fieldName, flags) ?? type.GetField(fieldName, flags);
            return field;
        }

        private object GetValue<T>(object obj, string fieldName)
        {
            var info = GetField<T>(fieldName, obj.GetType());
            return info?.GetValue(obj);
        }

        private void SetValue<T>(object obj, string fieldName, object value)
        {
            var field = GetField<T>(fieldName, obj.GetType());
            field?.SetValue(obj, value);
        }

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
            return field == null ? 0 : array.FindIndex(type => type.GetType() == field.GetType());
        }
    }
}