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
            if (!_interactable)
                return;

            var pos = Register.SpriteCanvas.Camera.ScreenToWorldPoint(mousePosition);
            pos = new Vector3(pos.x, pos.y, _spriteRenderer.bounds.center.z);

            if (_spriteRenderer.bounds.Contains(pos))
            {
                action?.Invoke();
            }
        }

        public override void SetUIElementProperties(UIElementSettings elementProperties)
        {
            base.SetUIElementProperties(elementProperties);
            _interactable = elementProperties.Interactable;
        }
    }
}