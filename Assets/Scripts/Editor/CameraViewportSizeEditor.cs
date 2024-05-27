using System.Reflection;
using Core.UI;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SpriteCanvasTest))]
    public class CameraViewportSizeEditor : UnityEditor.Editor
    {
        private readonly BindingFlags _bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private float _viewportWidth;
        private float _viewportHeight;
        private float _planeDistance;
        private Camera _camera;

        private void OnSceneGUI()
        {
            var isCalculateViewportSize = CalculateViewportSize();

            if (!isCalculateViewportSize)
                return;

            GetPrivateFields();
            if (_camera == null || _planeDistance == 0) return;

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

        private void GetPrivateFields()
        {
            var cvs = (SpriteCanvasTest)target;

            var planeDistanceField = cvs.GetType().GetField("_planeDistance", _bindingFlags);
            var cameraField = cvs.GetType().GetField("_camera", _bindingFlags);

            _viewportWidth = cvs.ViewportWidth;
            _viewportHeight = cvs.ViewportHeight;
            if (planeDistanceField != null) _planeDistance = (float)planeDistanceField.GetValue(cvs);
            if (cameraField != null) _camera = (Camera)cameraField.GetValue(cvs);
        }

        private bool CalculateViewportSize()
        {
            var cvs = (SpriteCanvasTest)target;
            var calculateViewportSizeMethod = cvs.GetType().GetMethod("CalculateCameraProp", _bindingFlags);

            if (calculateViewportSizeMethod == null)
                return false;

            calculateViewportSizeMethod.Invoke(cvs, null);
            return true;
        }
    }
}