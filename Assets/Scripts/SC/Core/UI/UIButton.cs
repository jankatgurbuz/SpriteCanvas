using SC.Core.Helper;
using UnityEngine;
using UnityEngine.Events;

namespace SC.Core.UI
{
    public class UIButton : UISprite
    {
        public UnityEvent DownEvent;
        public UnityEvent ClickEvent;

        private bool _interactable;

        public void Down(Vector3 mousePosition)
        {
            Action(mousePosition, DownEvent);
        }

        public void Up(Vector3 mousePosition)
        {
            Action(mousePosition, ClickEvent);
        }
        
        private void Action(Vector3 mousePosition, UnityEvent action)
        {
            if (!_interactable) return;

            var canvasCamera = Register.SpriteCanvas.CameraMode;
            var spriteCenter = _spriteRenderer.bounds.center;

            if (!canvasCamera.IsFakeCamera)
            {
                var distance = canvasCamera.Camera.orthographic
                    ? canvasCamera.Camera.WorldToScreenPoint(spriteCenter).z
                    : Vector3.Dot(spriteCenter - canvasCamera.Transform.position, canvasCamera.Transform.forward);
                mousePosition.z = distance;

                var worldPosition = canvasCamera.Camera.ScreenToWorldPoint(mousePosition);
                if (!canvasCamera.Camera.orthographic) worldPosition.z = spriteCenter.z;
                
                if (_spriteRenderer.bounds.Contains(worldPosition))
                {
                    action?.Invoke();
                }
            }
        }

        public override void SetUIElementProperties(UIElementSettings elementProperties)
        {
            base.SetUIElementProperties(elementProperties);
            _interactable = elementProperties.Interactable;
        }
    }
}