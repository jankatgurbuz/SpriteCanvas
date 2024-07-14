using System;
using System.Reflection;
using SC.Core.Helper.SpriteCanvasHelper;
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
            //EditorApplication.update += SpriteCanvasUpdater.OnEditorUpdate;
        }

        private void OnDisable()
        {
            //EditorApplication.update -= SpriteCanvasUpdater.OnEditorUpdate;
        }

        public override void OnInspectorGUI()
        {
            //DrawPropertiesExcluding(serializedObject, "m_Script", CanvasKey);

            var prop = serializedObject.GetIterator();
            var child = true;
            while (prop.NextVisible(child))
            {
                child = false;

                if (prop.name == "m_Script") continue;

                if (prop.name == "_runOnAwake")
                {
                    
                    EditorGUILayout.PropertyField(prop, true);
                    
                }
                
                if (prop.name == "_isVirtualCamera")
                {
                    DrawLine(1,1,1); 
                    EditorGUILayout.PropertyField(prop, true);
                  
                }

                if (prop.name == "_camera")
                {
                    var check = (bool)GetValue<SpriteCanvas>(target, "_isVirtualCamera");
                    if (!check)
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }
                }
                
                if (prop.name == "_virtualCamera")
                {
                    var check = (bool)GetValue<SpriteCanvas>(target, "_isVirtualCamera");
                    if (check)
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }
                }
                if (prop.name == "_planeDistance")
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
                if (prop.name == "_canvasScaler")
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
                
                
                if (prop.name == "_sortingLayerName")
                {
                    DrawLine(3,3,3); 
                    EditorGUILayout.PropertyField(prop, true);
                }
                if (prop.name == "_sortingLayerOrder")
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
             
                
                if (prop.name == "_uIElementProperties")
                {
                    DrawLine(3,3,3);
                    EditorGUILayout.PropertyField(prop, true);
                }
                if (prop.name == "_uiElements")
                {
                    DrawLine(3,3,3);
                    EditorGUILayout.PropertyField(prop, true);
                }   
            }
            //  DrawPropertiesExcluding(serializedObject, "m_Script", CanvasKey);

            DrawKeyArea();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawKeyArea()
        {
            if (_stringList == null)
            {
                EditorGUILayout.HelpBox("KeyContainer not found in Resources.", MessageType.Error);
                return;
            }

            _showKeyArea = EditorGUILayout.Toggle("Show Key Area", _showKeyArea);
            if (_showKeyArea)
            {
                DisableKeyArea();
                AddCustomKey();
                ListElements();
                PageNavigation();
                EditorGUILayout.Space();
            }
        }

        private void DisableKeyArea()
        {
            DrawLine(3, 3, 3);
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Canvas Key:", (string)GetValue<SpriteCanvas>(target, CanvasKey));
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Clear", GUI.skin.button))
            {
                SetValue<SpriteCanvas>(target, CanvasKey, string.Empty);
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();

            DrawLine(3, 3, 3);
        }

        private void AddCustomKey()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            var uICanvas = (SpriteCanvas)target;

            EditorGUILayout.LabelField("Add New Custom Key");

            _newKey = EditorGUILayout.TextField("New Key", _newKey);

            if (GUILayout.Button("Add Custom Key") && !string.IsNullOrEmpty(_newKey))
            {
                if (_stringList.SpriteCanvasKey.Contains(_newKey)) return;

                _stringList.SpriteCanvasKey.Add(_newKey);
                _currentPage = (_stringList.SpriteCanvasKey.Count - 1) / ItemsPerPage;
                SetValue<SpriteCanvas>(target, CanvasKey, _newKey);
                EditorUtility.SetDirty(uICanvas);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void ListElements()
        {
            DrawLine(3, 3, 3);
            EditorGUILayout.LabelField("-Select Key From List-", EditorStyles.boldLabel);

            if (_stringList.SpriteCanvasKey is not { Count: > 0 }) return;

            var startItem = _currentPage * ItemsPerPage;
            var endItem = Mathf.Min(startItem + ItemsPerPage, _stringList.SpriteCanvasKey.Count);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int i = startItem; i < endItem; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(_stringList.SpriteCanvasKey[i], EditorStyles.miniButtonRight))
                {
                    SetValue<SpriteCanvas>(target, CanvasKey, _stringList.SpriteCanvasKey[i]);
                    EditorUtility.SetDirty(target);
                }

                if (GUILayout.Button("-", EditorStyles.miniButton))
                {
                    _stringList.SpriteCanvasKey.RemoveAt(i);
                    EditorUtility.SetDirty(_stringList);
                    break;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void PageNavigation()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);

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

        private void DrawLine(int height, int topSpace, int bottomSpace)
        {
            EditorGUILayout.Space(topSpace);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));
            EditorGUILayout.Space(bottomSpace);
        }
    }

    public partial class SpriteCanvasEditor
    {
        private void OnSceneGUI()
        {
            _planeDistance = (float)GetValue<SpriteCanvas>(target, PlaneDistanceFieldName);
            _camera = (Camera)GetValue<SpriteCanvas>(target, CameraFieldName);
            var isVirtualCamera = (bool)GetValue<SpriteCanvas>(target, "_isVirtualCamera");
            if (!isVirtualCamera)
            {
                DrawCamera();
            }
            else
            {
                DrawVirtualCamera();
            }
        }

        private void DrawCamera()
        {
            if (_camera == null || _planeDistance == 0) return;

            var topLeft = _camera.ViewportToWorldPoint(new Vector3(0, 1, _planeDistance));
            var topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _planeDistance));
            var bottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _planeDistance));
            var bottomRight = _camera.ViewportToWorldPoint(new Vector3(1, 0, _planeDistance));

            const int thickness = 2;
            Handles.color = new Color32(255, 128, 0, 255);

            Handles.DrawLine(topLeft, topRight, thickness);
            Handles.DrawLine(topRight, bottomRight, thickness);
            Handles.DrawLine(bottomRight, bottomLeft, thickness);
            Handles.DrawLine(bottomLeft, topLeft, thickness);

            var cameraTopLeft = _camera.ViewportToWorldPoint(new Vector3(0, 1, 0));
            var cameraTopRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
            var cameraBottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            var cameraBottomRight = _camera.ViewportToWorldPoint(new Vector3(1, 0, 0));

            Handles.DrawLine(topLeft, cameraTopLeft, thickness);
            Handles.DrawLine(topRight, cameraTopRight, thickness);
            Handles.DrawLine(bottomLeft, cameraBottomLeft, thickness);
            Handles.DrawLine(bottomRight, cameraBottomRight, thickness);

            Handles.color = Color.white;
            Handles.DrawLine(cameraTopLeft, cameraTopRight, thickness - 1);
            Handles.DrawLine(cameraTopRight, cameraBottomRight, thickness - 1);
            Handles.DrawLine(cameraBottomRight, cameraBottomLeft, thickness - 1);
            Handles.DrawLine(cameraBottomLeft, cameraTopLeft, thickness - 1);
        }

        private void DrawVirtualCamera()
        {
            if (_planeDistance == 0) return;

            var fakeCamera = (VirtualCamera)GetValue<SpriteCanvas>(target, "_virtualCamera");

            var topLeft = fakeCamera.ViewportToWorldPoint(new Vector3(0, 1, _planeDistance));
            var topRight = fakeCamera.ViewportToWorldPoint(new Vector3(1, 1, _planeDistance));
            var bottomLeft = fakeCamera.ViewportToWorldPoint(new Vector3(0, 0, _planeDistance));
            var bottomRight = fakeCamera.ViewportToWorldPoint(new Vector3(1, 0, _planeDistance));

            const int thickness = 2;
            Handles.color = Color.cyan;

            Handles.DrawLine(topLeft, topRight, thickness);
            Handles.DrawLine(topRight, bottomRight, thickness);
            Handles.DrawLine(bottomRight, bottomLeft, thickness);
            Handles.DrawLine(bottomLeft, topLeft, thickness);

            var cameraTopLeft = fakeCamera.ViewportToWorldPoint(new Vector3(0, 1, 0));
            var cameraTopRight = fakeCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
            var cameraBottomLeft = fakeCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            var cameraBottomRight = fakeCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));

            Handles.DrawLine(topLeft, cameraTopLeft, thickness);
            Handles.DrawLine(topRight, cameraTopRight, thickness);
            Handles.DrawLine(bottomLeft, cameraBottomLeft, thickness);
            Handles.DrawLine(bottomRight, cameraBottomRight, thickness);
        }
    }
}