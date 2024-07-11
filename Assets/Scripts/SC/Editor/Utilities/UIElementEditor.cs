using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SC.Core.Helper.UIElementHelper;
using SC.Core.UI;
using SC.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Utilities
{
    [CustomEditor(typeof(UIElement), true)]
    public partial class UIElementEditor : UnityEditor.Editor
    {
        private const int ItemsPerPage = 5;
        private List<string> _keyList;
        private int _currentPage;

        private void OnEnable()
        {
            InitializeResponsiveOperation();
        }
        public override void OnInspectorGUI()
        {
            AssignComponent();
            FillResponsiveField();
            _keyList = CreateCanvasKeyList();
            DrawUIElements();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUIElements()
        {
            var prop = serializedObject.GetIterator();
            while (prop.NextVisible(true))
            {
                if (prop.name == "m_Script") continue;

                Draw(prop, "_responsiveOperation", line: true);
                Draw(prop, "_register", true, RegisterAction,line: true);
                Draw(prop, "_uIElementProperties", includeChildren: false,
                    ignoreChild: CheckParentGroup() ? "_useInitialCameraPosition" : "", line: true);
                Draw(prop, "_hasReference", line: true);
                Draw(prop, "_referenceElement", serializedObject.FindProperty("_hasReference").boolValue);
                Draw(prop, "_spriteSize");
                Draw(prop, "_textMeshProSize");
                Draw(prop, "DownEvent");
                Draw(prop, "ClickEvent");
            }

            DrawLine(3, 1, 1);
            var child = true;
            prop = serializedObject.GetIterator();
            while (prop.NextVisible(child))
            {
                child = false;
                GUI.enabled = false;
                Draw(prop, "_alpha");
                Draw(prop, "_itemPosition");
                Draw(prop, "_spriteRenderer");
                Draw(prop, "_textMeshPro");
                GUI.enabled = true;
            }
        }
        private void RegisterAction()
        {
            var t = GetValue<UIElement>(target, "_register") as RegisterProperties;

            if (!serializedObject.FindProperty("_register").isExpanded) return;
            if (t.RegisterType != RegisterType.Key) return;

            ListElements();
            PageNavigation();
        }

        private void DrawLine(int height, int topSpace, int bottomSpace)
        {
            EditorGUILayout.Space(topSpace);
            GUILayout.Box("test", GUILayout.ExpandWidth(true), GUILayout.Height(height));
            EditorGUILayout.Space(bottomSpace);
        }

        private bool CheckParentGroup()
        {
            var element = target as UIElement;
            var currentElement = element.ReferenceElement;
            while (currentElement != null)
            {
                if (currentElement.IsGroupChecked) return true;
                currentElement = currentElement.ReferenceElement;
            }

            return false;
        }

        private void Draw(SerializedProperty prop, string drawName, bool condition = true, Action action = null,
            bool line = false, bool includeChildren = true, params string[] ignoreChild)
        {
            if (prop.name != drawName || !condition) return;

            if (includeChildren)
            {
                if (line) DrawLine(2, 1, 1);
                EditorGUILayout.PropertyField(prop, true);
            }
            else
            {
                if (line) DrawLine(2, 1, 1);
                DrawPropertyWithoutChildren(prop, ignoreChild);
            }

            action?.Invoke();

            void DrawPropertyWithoutChildren(SerializedProperty property, string[] ignoreChildren)
            {
                var path = property.propertyPath;
                property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName);
                if (!property.isExpanded) return;
                while (property.NextVisible(true))
                {
                    if (!property.propertyPath.Contains(path)) break;

                    if (ignoreChildren.Length == 0 || !ignoreChildren.Contains(property.name))
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(property, true);
                        GUI.enabled = true;
                    }
                }
            }
        }

        private List<string> CreateCanvasKeyList()
        {
            var list = new List<string>();
            var sc = Resources.FindObjectsOfTypeAll<SpriteCanvas>();
            foreach (var spriteCanvas in sc)
            {
                var value = GetValue<SpriteCanvas>(spriteCanvas, "_canvasKey");

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
                    var t = GetValue<UIElement>(uICanvas, "_register") as RegisterProperties;
                    SetValue<RegisterProperties>(uICanvas.Register, "_canvasKey", _keyList[i]);
                    SetValue<UIElement>(uICanvas, "_register", t);

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
    }
}