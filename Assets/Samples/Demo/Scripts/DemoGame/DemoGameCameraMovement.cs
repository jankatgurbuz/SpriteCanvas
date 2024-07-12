using SC.Core.UI;
using UnityEngine;

namespace Samples.Demo.Scripts.DemoGame
{
    public class DemoGameCameraMovement : MonoBehaviour
    {
        [SerializeField] private SpriteCanvas _spriteCanvas;
        [SerializeField] private float _moveSpeed = 10f;

        private Vector3 _targetPosition;
        private bool _isMoving;
        private Vector3 _cameraInitPos;

        private void Start()
        {
            _targetPosition = transform.position;
            _cameraInitPos = _targetPosition;
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
            float newPositionX;
            switch (index)
            {
                case 0:
                    newPositionX = _cameraInitPos.x + -2 * _spriteCanvas.ViewportWidth;
                    break;
                case 4:
                    newPositionX = _cameraInitPos.x + 2 * _spriteCanvas.ViewportWidth;
                    break;
                case 2:
                    newPositionX = _cameraInitPos.x;
                    break;
                default:
                    newPositionX = _cameraInitPos.x + (index - 2) * _spriteCanvas.ViewportWidth;
                    break;
            }

            _targetPosition = new Vector3(newPositionX, transform.position.y, transform.position.z);
            _isMoving = true;
        }
    }
}