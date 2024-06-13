using System;
using System.Reflection;
using SC.Core.UI;
using SC.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Utilities
{
    [CustomEditor(typeof(SpriteCanvas), true)]
    public partial class SpriteCanvasEditor : UnityEditor.Editor
    {
        private const string CanvasKey = "_canvasKey";
        private const string KeyContainer = "KeyContainer";
        private const string PlaneDistanceFieldName = "_planeDistance";
        private const string CameraFieldName = "_camera";
        private const int ItemsPerPage = 5;

        private SOKeyContainer _stringList;
        private string _newKey = "";
        private int _currentPage;
        private bool _showKeyArea;
        private float _planeDistance;
        private Camera _camera;

        private void OnEnable()
        {
            _stringList = Resources.Load<SOKeyContainer>(KeyContainer);
            EditorApplication.update += SpriteCanvasUpdater.OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= SpriteCanvasUpdater.OnEditorUpdate;
        }

        public override void OnInspectorGUI()
        {
            if (_stringList == null)
            {
                EditorGUILayout.HelpBox("KeyContainer not found in Resources.", MessageType.Error);
                return;
            }

            DrawPropertiesExcluding(serializedObject, "m_Script", CanvasKey);
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
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Canvas Key:", (string)GetValue<SpriteCanvas>(target, CanvasKey));
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                SetValue<SpriteCanvas>(target, CanvasKey, string.Empty);
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ListElements()
        {
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
                    SetValue<SpriteCanvas>(target, CanvasKey, _stringList.SpriteCanvasKey[i]);
                    EditorUtility.SetDirty(target);
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
                SetValue<SpriteCanvas>(target, CanvasKey, _newKey);
                EditorUtility.SetDirty(uICanvas);
            }
        }

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

    public partial class SpriteCanvasEditor
    {
        private void OnSceneGUI()
        {
            _planeDistance = (float)GetValue<SpriteCanvas>(target, PlaneDistanceFieldName);
            _camera = (Camera)GetValue<SpriteCanvas>(target, CameraFieldName);

            if (_camera == null || _planeDistance == 0) return;

            Draw();
        }

        private void Draw()
        {
            var topLeft = _camera.ViewportToWorldPoint(new Vector3(0, 1, _planeDistance));
            var topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _planeDistance));
            var bottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _planeDistance));
            var bottomRight = _camera.ViewportToWorldPoint(new Vector3(1, 0, _planeDistance));

            Handles.color = Color.red;

            Handles.DrawLine(topLeft, topRight);
            Handles.DrawLine(topRight, bottomRight);
            Handles.DrawLine(bottomRight, bottomLeft);
            Handles.DrawLine(bottomLeft, topLeft);

            var cameraTopLeft = _camera.ViewportToWorldPoint(new Vector3(0, 1, 0));
            var cameraTopRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
            var cameraBottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            var cameraBottomRight = _camera.ViewportToWorldPoint(new Vector3(1, 0, 0));

            Handles.DrawLine(topLeft, cameraTopLeft);
            Handles.DrawLine(topRight, cameraTopRight);
            Handles.DrawLine(bottomLeft, cameraBottomLeft);
            Handles.DrawLine(bottomRight, cameraBottomRight);
        }
    }
}