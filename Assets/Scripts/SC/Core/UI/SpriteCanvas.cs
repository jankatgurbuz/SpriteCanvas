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
            UpdateCameraViewportProperties();
        }

        private void CreateSpriteCanvasManager()
        {
            if (_spriteCanvasManager != null) return;

            var managerObj = new GameObject("SpriteCanvasManager");
            _spriteCanvasManager = managerObj.AddComponent<SpriteCanvasManager>();
            DontDestroyOnLoad(managerObj);

            _spriteCanvasManager.Initialize();
        }

        private void UpdateCameraViewportProperties()
        {
            if (_camera == null) return;
            ViewportHeight = _camera.orthographic
                ? 2f * _camera.orthographicSize
                : 2.0f * _planeDistance * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            ViewportWidth = ViewportHeight * _camera.aspect;
            Balance = _canvasScaler == CanvasScaler.Height ? ViewportHeight * 0.1f : ViewportWidth * 0.1f;
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
    }
}