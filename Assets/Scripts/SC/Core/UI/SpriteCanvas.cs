using System.Collections.Generic;
using SC.Core.Manager;
using SC.Core.SpriteCanvasAttribute;
using UnityEngine;

namespace SC.Core.UI
{
    public class SpriteCanvas : MonoBehaviour
    {
        [SerializeField] private string _canvasKey;

        [SerializeField] private Camera _camera;

        [SerializeField] private float _planeDistance = 10;

        [SCHorizontalLine(EColor.Orange), SerializeField, SCSortingLayer]
        private string _sortingLayerName;

        [SerializeField] private int _sortingLayerOrder;

        [SerializeField] private CanvasScaler _canvasScaler;

        [SerializeField] private UIElementProperties _uIElementProperties;

        public UIElementProperties ElementProperties => _uIElementProperties;
        public Camera Camera => _camera;
        public string SortingLayerName => _sortingLayerName;
        public int SortingLayerOrder => _sortingLayerOrder;
        public float ViewportHeight { get; private set; }
        public float ViewportWidth { get; private set; }
        public Vector3 ViewportPosition { get; private set; }
        public float Balance { get; private set; }

        private static SpriteCanvasManager _spriteCanvasManager;

        private List<UIElement> _uiElements;

        private void Awake()
        {
            _uiElements = new List<UIElement>();
            CreateSpriteCanvasManager();
            _spriteCanvasManager.SpriteCanvasRegister(_canvasKey, this);
            CalculateCameraProp();
        }

        private void CreateSpriteCanvasManager()
        {
            if (_spriteCanvasManager != null) return;

            var managerObj = new GameObject("SpriteCanvasManager");
            _spriteCanvasManager = managerObj.AddComponent<SpriteCanvasManager>();
            DontDestroyOnLoad(managerObj);

            _spriteCanvasManager.Initialize();
        }

        private void CalculateCameraProp()
        {
            if (_camera == null)
            {
                return;
            }

            if (_camera.orthographic)
            {
                ViewportHeight = 2f * _camera.orthographicSize;
                ViewportWidth = ViewportHeight * _camera.aspect;
                Balance = _camera.orthographicSize / 5f;
            }
            else
            {
                ViewportHeight = 2.0f * _planeDistance * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                ViewportWidth = ViewportHeight * _camera.aspect;
                Balance = _canvasScaler == CanvasScaler.Height ? ViewportHeight / 10f : ViewportWidth / 10;
            }

            ViewportPosition = GetViewportCenterPosition();
        }

        private Vector3 GetViewportCenterPosition()
        {
            var viewportCenter = new Vector3(0.5f, 0.5f, _planeDistance);
            return _camera.ViewportToWorldPoint(viewportCenter);
        }

        public void ShowAllUIs()
        {
            _uIElementProperties.Alpha = 1;
            _uIElementProperties.Interactable = true;

            foreach (var item in _uiElements)
            {
                item.SetUIElementProperties(_uIElementProperties);
            }
        }

        public void HideAllUIs()
        {
            _uIElementProperties.Alpha = 0;
            _uIElementProperties.Interactable = false;

            foreach (var item in _uiElements)
            {
                item.SetUIElementProperties(_uIElementProperties);
            }
        }

        public void AddUI(UIElement ui)
        {
            _uiElements.Add(ui);
        }

        public enum CanvasScaler
        {
            Height,
            Width
        }

        [System.Serializable]
        public class UIElementProperties
        {
            [Range(0f, 1f)] public float Alpha = 1;

            public bool Interactable = true;
        }

#if UNITY_EDITOR
        public void UpdateForEditor()
        {
            if (Application.isPlaying) return;
            CalculateCameraProp();
            var objects = FindObjectsOfType<UIElement>();
            foreach (var item in objects)
            {
                var field = typeof(UIElement).GetField("_registerType",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                var value = (UIElement.RegisterType)field.GetValue(item);

                switch (value)
                {
                    case UIElement.RegisterType.Hierarchy:
                        SpriteCanvas sc = null;
                        var currentParent = item.transform.parent;

                        while (currentParent != null)
                        {
                            if (currentParent.TryGetComponent(out SpriteCanvas spriteCanvas))
                                sc = spriteCanvas;

                            currentParent = currentParent.parent;
                        }

                        if (sc == null)
                        {
                            continue;
                        }

                        if (sc != this)
                        {
                            continue;
                        }

                        break;
                    case UIElement.RegisterType.Reference:
                        var r = typeof(UIElement).GetField("_spriteCanvas",
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                        var reference = (SpriteCanvas)r.GetValue(item);


                        if (reference == null)
                        {
                            continue;
                        }

                        if (reference != this)
                        {
                            continue;
                        }

                        break;
                    case UIElement.RegisterType.Key:
                        if (item.CanvasKey != _canvasKey) continue;
                        break;
                }

                item.ArrangeLayers(_sortingLayerName, _sortingLayerOrder);
                item.SetUILayout(ViewportHeight, ViewportWidth, ViewportPosition, Balance);
                item.SetUIElementProperties(ElementProperties);
            }
        }
#endif
    }
}