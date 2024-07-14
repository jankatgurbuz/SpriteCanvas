using System.Collections.Generic;
using SC.Core.Helper;
using SC.Core.Helper.SpriteCanvasHelper;
using SC.Core.Manager;
using SC.Core.SpriteCanvasAttribute;
using UnityEngine;

namespace SC.Core.UI
{
    public class SpriteCanvas : MonoBehaviour
    {
        [SerializeField] private string _canvasKey;
        [SerializeField] private bool _runOnAwake = true;
        [SerializeField] private bool _isVirtualCamera;
        [SerializeField] private VirtualCamera _virtualCamera;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _planeDistance = 10;
        [SerializeField] private CanvasScaler _canvasScaler;
        [SerializeField, SortingLayer] private string _sortingLayerName;
        [SerializeField] private int _sortingLayerOrder;
        [SerializeField] private UIElementSettings _uIElementProperties;
        [SerializeField, ReadOnly] private List<UIElement> _uiElements;

        private static SpriteCanvasManager _spriteCanvasManager;

        public UIElementSettings ElementProperties => _uIElementProperties;
        public CameraMode CameraMode { get; private set; }
        public string SortingLayerName => _sortingLayerName;
        public string CanvasKey => _canvasKey;
        public int SortingLayerOrder => _sortingLayerOrder;
        public float ViewportHeight { get; private set; }
        public float ViewportWidth { get; private set; }
        public Vector3 ViewportPosition { get; private set; }
        public float Balance { get; private set; }

        private void Awake()
        {
            CreateSpriteCanvasManager();
            _spriteCanvasManager.SpriteCanvasRegister(_canvasKey, this);
            AdjustCameraMode();

            if (!_runOnAwake) return;

            AdjustDependentUIElements();
        }
        private void RemoveNullElements()
        {
            var initialCount = _uiElements.Count;
            _uiElements.RemoveAll(item => item == null);
            var removedCount = initialCount - _uiElements.Count;

            if (removedCount > 0)
            {
                Debug.LogWarning($"{removedCount} null UI elements were removed from the list.");
            }
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
            if (_isVirtualCamera)
            {
                ViewportHeight = 2f * CameraMode.VirtualCamera.OrthographicSize;
                ViewportWidth = ViewportHeight * CameraMode.VirtualCamera.AspectRatio;
                Balance = _canvasScaler == CanvasScaler.Height ? ViewportHeight * 0.1f : ViewportWidth * 0.1f;

                var viewportCenter = new Vector3(0.5f, 0.5f, _planeDistance);
                ViewportPosition = CameraMode.VirtualCamera.ViewportToWorldPoint(viewportCenter);
            }
            else
            {
                if (_camera == null) return;

                ViewportHeight = _camera.orthographic
                    ? 2f * _camera.orthographicSize
                    : 2.0f * _planeDistance * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

                ViewportWidth = ViewportHeight * _camera.aspect;
                Balance = _canvasScaler == CanvasScaler.Height ? ViewportHeight * 0.1f : ViewportWidth * 0.1f;

                var viewportCenter = new Vector3(0.5f, 0.5f, _planeDistance);
                ViewportPosition = _camera.ViewportToWorldPoint(viewportCenter);
            }
        }

        private bool CheckCamera()
        {
            if (_isVirtualCamera)
            {
                if (_virtualCamera.Transform == null)
                {
                    Debug.LogWarning("VirtualCamera.Transform is null");
                    return false;
                }
            }
            else
            {
                if (_camera == null)
                {
                    Debug.LogWarning("Camera is null");
                    return false;
                }
            }

            return true;
        }

        public void AdjustDependentUIElements()
        {
            if (!CheckCamera()) return;

            RemoveNullElements();
            UpdateCameraViewportProperties();
            _uiElements.ForEach(x => x.ResetFlags());
            _uiElements.ForEach(x => x.Adjust());
        }
        
        public void AdjustCameraMode()
        {
            if (!CheckCamera()) return;

            CameraMode = new CameraMode();
            if (_isVirtualCamera)
            {
                CameraMode.SetMode(true, virtualCamera: _virtualCamera, planeDistance: _planeDistance);
            }
            else
            {
                CameraMode.SetMode(false, camera: _camera);
            }
        }

        public void SetCamera(Camera camera, bool runAdjustMethods = true)
        {
            _camera = camera;
            _isVirtualCamera = false;

            if (!runAdjustMethods) return;

            AdjustCameraMode();
            AdjustDependentUIElements();
        }

        public void SetVirtualCamera(VirtualCamera virtualCamera = null, bool runAdjustMethods = true)
        {
            if (virtualCamera != null)
            {
                _virtualCamera = virtualCamera;
            }

            _isVirtualCamera = true;

            if (!runAdjustMethods) return;

            AdjustCameraMode();
            AdjustDependentUIElements();
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
            if (_uiElements.Contains(ui)) return;

            _uiElements.Add(ui);
        }
    }
}