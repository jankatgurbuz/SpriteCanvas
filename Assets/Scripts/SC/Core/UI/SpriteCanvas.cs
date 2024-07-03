using System.Collections.Generic;
using System.Linq;
using SC.Core.Helper;
using SC.Core.Manager;
using SC.Core.SpriteCanvasAttribute;
using UnityEngine;
using EColor = SC.Core.SpriteCanvasAttribute.EColor;

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

        [SerializeField] private UIElementSettings _uIElementProperties;
        [SerializeField, ReadOnly] private List<UIElement> _uiElements;
        public UIElementSettings ElementProperties => _uIElementProperties;
        public Camera Camera => _camera;
        public string SortingLayerName => _sortingLayerName;
        public string CanvasKey => _canvasKey;
        public int SortingLayerOrder => _sortingLayerOrder;
        public float ViewportHeight { get; private set; }
        public float ViewportWidth { get; private set; }
        public Vector3 ViewportPosition { get; private set; }
        public float Balance { get; private set; }

        private static SpriteCanvasManager _spriteCanvasManager;

        private enum CanvasScaler
        {
            Height,
            Width
        }

        private void Awake()
        {
            CreateSpriteCanvasManager();
            _spriteCanvasManager.SpriteCanvasRegister(_canvasKey, this);
            AdjustDependentUIElements();
        }

        public void AdjustDependentUIElements()
        {
            RemoveNullElements();
            UpdateCameraViewportProperties();
            _uiElements.ForEach(x => x.ResetFlags());
            _uiElements.ForEach(x => x.Adjust());
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
            if (_uiElements.Contains(ui)) return;
            _uiElements.Add(ui);
        } 
    } 
}