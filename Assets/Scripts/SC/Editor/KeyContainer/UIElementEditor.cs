using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SC.Core.ResponsiveOperations;
using SC.Core.UI;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.KeyContainer
{
    [CustomEditor(typeof(UIElement), true)]
    public class UIElementEditor : UnityEditor.Editor
    {
        private const string _responsiveOperationFieldName = "_responsiveOperation";
        private const string _itemPositionFieldName = "_itemPosition";
        private const string _spriteRendererFieldName = "_spriteRenderer";
        private const string _textMeshProFieldName = "_textMeshPro";
        private const int ItemsPerPage = 5;

        private readonly BindingFlags _bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private List<string> _keyList;
        private List<IResponsiveOperation> _implementingTypes;
        private string[] _typeNames;

        private Transform _itemPosition;
        private int _currentPage;
        private bool _initFlag;

        private void OnEnable()
        {
            _initFlag = false;
            _implementingTypes = GenerateTypeInstances<IResponsiveOperation>();
            _typeNames = ConvertTypeListToArray(_implementingTypes);
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
                if (prop.name != "_spriteCanvas") continue;

                var value = (UIElement.RegisterType)GetValue<UIElement>(target, "_registerType");
                if (value != UIElement.RegisterType.Key) continue;
                ListElements();
                PageNavigation();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AssignComponent()
        {
            AssignComponent<Transform>(_itemPositionFieldName);
            AssignComponent<SpriteRenderer>(_spriteRendererFieldName);
            AssignComponent<TextMeshPro>(_textMeshProFieldName);

            void AssignComponent<T>(string fieldName) where T : Component
            {
                var currentValue = GetValue<T>(target, fieldName);
                var component = ((UIElement)target).GetComponent<T>();
                if (component != null && currentValue == null)
                {
                    SetValue<UIElement>(target, fieldName, component);
                }
            }
        }

        private void FillResponsiveField()
        {
            var data = GetValue<UIElement>(target, _responsiveOperationFieldName);
            var currentIndex = FindTypeIndexInArray(_implementingTypes, (ResponsiveOperation)data);

            EditorGUI.BeginChangeCheck();
            var selectedIndex = EditorGUILayout.Popup("Responsive", currentIndex, _typeNames);
            if (!_initFlag || EditorGUI.EndChangeCheck())
            {
                var spriteUIOperationHandler = _implementingTypes[selectedIndex];

                if (data == null || data.GetType() != spriteUIOperationHandler.GetType())
                {
                    SetValue<UIElement>(target, _responsiveOperationFieldName, spriteUIOperationHandler);
                }

                EditorUtility.SetDirty(target);
                _initFlag = true;
            }
        }

        private List<string> CreateCanvasKeyList()
        {
            var sc = Resources.FindObjectsOfTypeAll<SpriteCanvas>();
            var list = new List<string>();
            foreach (var spriteCanvas in sc)
            {
                var field = spriteCanvas.GetType()
                    .GetField("_canvasKey", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null) continue;

                var value = field.GetValue(spriteCanvas);

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
                    SetValue<UIElement>(target, "_canvasKey", _keyList[i]);
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
            var field = typeof(T).GetField(fieldName, _bindingFlags) ?? type.GetField(fieldName, _bindingFlags);
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
            if (field == null)
                return 0;

            return array.FindIndex(type => type.GetType() == field.GetType());
        }
    }
}