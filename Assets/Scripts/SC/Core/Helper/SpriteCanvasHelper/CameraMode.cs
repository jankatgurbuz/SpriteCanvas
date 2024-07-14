using UnityEngine;

namespace SC.Core.Helper.SpriteCanvasHelper
{
    public class CameraMode
    {
        public bool IsFakeCamera { get; private set; }
        public VirtualCamera VirtualCamera { get; private set; }
        public Transform Transform { get; private set; }
        public Camera Camera { get; private set; }

        public void SetMode(bool isFakeCamera, Camera camera = null, VirtualCamera virtualCamera = null,
            float planeDistance = 10)
        {
            IsFakeCamera = isFakeCamera;
            Camera = camera;
            VirtualCamera = virtualCamera;
            if (IsFakeCamera)
            {
                Transform = VirtualCamera?.Transform;
                VirtualCamera.PlaneDistance = planeDistance * 0.1f;
            }
            else
            {
                Transform = Camera.transform;
            }
        }
    }
}