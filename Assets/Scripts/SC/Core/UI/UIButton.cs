using SC.Manager;
using UnityEngine;
using UnityEngine.Events;

namespace SC.Core.UI
{
    public class UIButton : UISprite
    {
        public UnityEvent DownEvent;
        public UnityEvent ClickEvent;
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
            var pos = _spriteCanvas.Camera.ScreenToWorldPoint(mousePosition);
            pos = new Vector3(pos.x, pos.y, _spriteRenderer.bounds.center.z);

            if (_spriteRenderer.bounds.Contains(pos))
            {
                action?.Invoke();
            }
        }
    }
}