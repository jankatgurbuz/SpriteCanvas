using System.Collections.Generic;
using System.Reflection;
using SC.Core.UI;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.KeyContainer
{
    [CustomEditor(typeof(UIElement), true)]
    public class UIElementEditor : UnityEditor.Editor
    {
        private List<string> _keyList;
        private int _currentPage;
        private const int ItemsPerPage = 5;

        public override void OnInspectorGUI()
        {
            _keyList = CreateCanvasKeyList();

            serializedObject.Update();

            var prop = serializedObject.GetIterator();
            var enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                EditorGUILayout.PropertyField(prop, true);
                if (prop.name != "_spriteCanvas") continue;
                if (GetRegisterType() != UIElement.RegisterType.Key) continue;
                ListElements();
                PageNavigation();
            }

            serializedObject.ApplyModifiedProperties();
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
                    SetCanvasKey(_keyList[i]);
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

        private FieldInfo GetCanvasKeyField()
        {
            return typeof(UIElement).GetField("_canvasKey", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        private void SetCanvasKey(string value)
        {
            var field = GetCanvasKeyField();
            field.SetValue(target, value);
        }

        private FieldInfo GetRegisterTypeField()
        {
            return typeof(UIElement).GetField("_registerType", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private UIElement.RegisterType GetRegisterType()
        {
            var field = GetRegisterTypeField();
            return (UIElement.RegisterType)field.GetValue(target);
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
    }
}