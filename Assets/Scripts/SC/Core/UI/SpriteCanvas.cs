using SC.Manager;
using SC.SpriteCanvasAttribute;
using UnityEngine;

namespace SC.Core.UI
{
    public class SpriteCanvas : MonoBehaviour
    {
        [SerializeField] private string _canvasKey;

        [SerializeField] private Camera _camera;

        [SerializeField] private float _planeDistance;

        [SCHorizontalLine(EColor.Orange), SerializeField, SCSortingLayer]
        private string _sortingLayerName;

        [SerializeField] private int _sortingLayerOrder;

        [SerializeField] private CanvasScaler _canvasScaler;

        public Camera Camera => _camera;
        public string SortingLayerName => _sortingLayerName;
        public int SortingLayerOrder => _sortingLayerOrder;
        public float ViewportHeight { get; private set; }
        public float ViewportWidth { get; private set; }

        public Vector3 ViewportPosition { get; private set; }
        public float Balance { get; private set; }

        private static SpriteCanvasManager _spriteCanvasManager;

        private void Awake()
        {
            CreateSpriteCanvasManager();
            _spriteCanvasManager.Register(_canvasKey, this);
            CalculateCameraProp();
        }

        private void CalculateCameraProp()
        {
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

        private void CreateSpriteCanvasManager()
        {
            if (_spriteCanvasManager != null) return;

            var managerObj = new GameObject("SpriteCanvasManager");
            _spriteCanvasManager = managerObj.AddComponent<SpriteCanvasManager>();
            Object.DontDestroyOnLoad(managerObj);
        }

        private Vector3 GetViewportCenterPosition()
        {
            var viewportCenter = new Vector3(0.5f, 0.5f, _planeDistance);
            return _camera.ViewportToWorldPoint(viewportCenter);
        }

        public enum CanvasScaler
        {
            Height,
            Width
        }

#if UNITY_EDITOR
        public void UpdateForEditor()
        {
            if (Application.isPlaying) return;
            CalculateCameraProp();
            var objects = FindObjectsOfType<UIElement>();
            foreach (var item in objects)
            {
                if (item.CanvasKey != _canvasKey) continue;
                item.ArrangeLayers(_sortingLayerName, _sortingLayerOrder);
                item.SetUILayout(ViewportHeight, ViewportWidth, ViewportPosition, Balance);
            }
        }
#endif
    }
}