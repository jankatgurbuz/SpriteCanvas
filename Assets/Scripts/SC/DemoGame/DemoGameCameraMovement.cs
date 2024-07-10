using SC.Core.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace SC.DemoGame
{
    public class DemoGameCameraMovement : MonoBehaviour
    {
        [SerializeField] private SpriteCanvas _spriteCanvas;
        [SerializeField] private float _moveSpeed = 10f;
        
        private Vector3 _targetPosition;
        private bool _isMoving = false;

        private void Start()
        {
            _targetPosition = transform.position;
        }

        private void Update()
        {
            if (!_isMoving) return;
            transform.position =
                Vector3.Lerp(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                transform.position = _targetPosition;
                _isMoving = false;
            }

            _spriteCanvas.AdjustDependentUIElements();
        }

        public void OnItemSelectionChanged(int index)
        {
            var newPositionX = index switch
            {
                0 => -2 * _spriteCanvas.ViewportWidth,
                4 => 2 * _spriteCanvas.ViewportWidth,
                _ => (index - 2) * _spriteCanvas.ViewportWidth
            };

            _targetPosition = new Vector3(newPositionX, transform.position.y, transform.position.z);
            _isMoving = true;
        }
    }
}