using UnityEngine;

namespace SC.Core.Helper.SpriteCanvasHelper
{
    [System.Serializable]
    public class VirtualCamera
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float _orthographicSize = 5f;
        [SerializeField] private float _screenWidth = 1920f;
        [SerializeField] private float _screenHeight = 1080f;
        public Transform Transform => _transform;
        public float OrthographicSize => _orthographicSize;
        public float ScreenWidth => _screenWidth;
        public float ScreenHeight => _screenHeight;
        public float AspectRatio => _screenWidth / _screenHeight;

        public float PlaneDistance { get; set; }
        public Vector3 ViewportToWorldPoint(Vector3 viewportPoint)
        {
            if (_transform == null) return Vector3.zero;

            var x = (viewportPoint.x - 0.5f) * 2 * _orthographicSize * AspectRatio;
            var y = (viewportPoint.y - 0.5f) * 2 * _orthographicSize;
            var z = viewportPoint.z * PlaneDistance;

            var cameraSpacePoint = new Vector3(x, y, z);
            var worldPoint = _transform.position + _transform.rotation * cameraSpacePoint;

            return worldPoint;
        }
    }
}
