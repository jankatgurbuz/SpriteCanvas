using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SC.Core.UI;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.KeyContainer
{
    [CustomEditor(typeof(SpriteCanvas))]
    public class SpriteCanvasEditor : UnityEditor.Editor
    {
        private SOKeyContainer _stringList;
        private string _newKey = "";
        private int _currentPage;
        private const int ItemsPerPage = 5;
        private bool _showKeyArea;

        private void OnEnable()
        {
            _stringList = Resources.Load<SOKeyContainer>("KeyContainer");
        }
        public override void OnInspectorGUI()
        {
            if (_stringList == null)
            {
                EditorGUILayout.HelpBox("KeyContainer not found in Resources.", MessageType.Error);
                return;
            }

            DrawPropertiesExcluding(serializedObject, "m_Script", "_canvasKey");
            EditorGUILayout.Space(5);

            _showKeyArea = EditorGUILayout.Toggle("Show Key Area", _showKeyArea);
            if (_showKeyArea)
            {
                DisableKeyArea();
                EditorGUILayout.Space();
                AddCustomKey();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                ListElements();
                PageNavigation();
                EditorGUILayout.Space();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DisableKeyArea()
        {
            var uICanvas = (SpriteCanvas)target;
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Canvas Key:", GetValue());
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                SetValue(string.Empty);
                EditorUtility.SetDirty(uICanvas);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ListElements()
        {
            var uICanvas = (SpriteCanvas)target;
            EditorGUILayout.LabelField("-Select Key From List-", EditorStyles.boldLabel);

            if (_stringList.SpriteCanvasKey is not { Count: > 0 }) return;

            var startItem = _currentPage * ItemsPerPage;
            var endItem = Mathf.Min(startItem + ItemsPerPage, _stringList.SpriteCanvasKey.Count);

            for (int i = startItem; i < endItem; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(_stringList.SpriteCanvasKey[i], GUILayout.Width(150), GUILayout.Height(25)))
                {
                    SetValue(_stringList.SpriteCanvasKey[i]);
                    EditorUtility.SetDirty(uICanvas);
                }

                if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(25)))
                {
                    _stringList.SpriteCanvasKey.RemoveAt(i);
                    EditorUtility.SetDirty(_stringList);
                    break;
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
                $"Page {_currentPage + 1}/{Mathf.CeilToInt((float)_stringList.SpriteCanvasKey.Count / ItemsPerPage)}",
                GUILayout.Width(100));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(">", GUILayout.Width(50)))
            {
                _currentPage = Mathf.Min(_currentPage + 1,
                    Mathf.CeilToInt((float)_stringList.SpriteCanvasKey.Count / ItemsPerPage) - 1);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void AddCustomKey()
        {
            var uICanvas = (SpriteCanvas)target;
            EditorGUILayout.LabelField("Add New Custom Key");
            _newKey = EditorGUILayout.TextField("New Key", _newKey);
            if (GUILayout.Button("Add Custom Key") && !string.IsNullOrEmpty(_newKey))
            {
                if (_stringList.SpriteCanvasKey.Contains(_newKey)) return;

                _stringList.SpriteCanvasKey.Add(_newKey);
                EditorUtility.SetDirty(_stringList);
                _currentPage = (_stringList.SpriteCanvasKey.Count - 1) / ItemsPerPage;
                SetValue(_newKey);
                EditorUtility.SetDirty(uICanvas);
            }
        }
        private FieldInfo GetField()
        {
            return typeof(SpriteCanvas).GetField("_canvasKey", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private string GetValue()
        {
            var field = GetField();
            return (string)field.GetValue(target);
        }

        private void SetValue(string value)
        {
            var field = GetField();
            field.SetValue(target, value);
        }
    }
}