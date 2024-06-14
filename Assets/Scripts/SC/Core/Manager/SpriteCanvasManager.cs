using System.Collections.Generic;
using SC.Core.UI;
using UnityEngine;

namespace SC.Core.Manager
{
    public class SpriteCanvasManager : MonoBehaviour
    {
        public static SpriteCanvasManager Instance;
        private Dictionary<string, SpriteCanvas> _spriteCanvasMap;
        private Dictionary<string, UIElement> _targetUI;
        private List<UIButton> _uiButtons;
        private void Awake()
        {
            Instance = this;
            _uiButtons = new List<UIButton>();
        }

        public void Initialize()
        {
            _spriteCanvasMap = new Dictionary<string, SpriteCanvas>();
        }

        public void SpriteCanvasRegister(string str, SpriteCanvas spriteCanvas)
        {
            if (str == string.Empty)
            {
                return;
            }
            if (!_spriteCanvasMap.TryAdd(str, spriteCanvas))
            {
                Debug.LogError($"Failed to register SpriteCanvas with key {str}. It might already exist in the map.");
            }
        }

        public SpriteCanvas GetSpriteCanvas(string key)
        {
            if (_spriteCanvasMap.TryGetValue(key, out var spriteCanvas))
            {
                return spriteCanvas;
            }

            Debug.LogError($"No SpriteCanvas found with key {key}.");
            return null;
        }

        public void RegisterTarget(string targetKey, UIElement uiElement)
        {
            _targetUI ??= new Dictionary<string, UIElement>();

            if (!string.IsNullOrEmpty(targetKey))
            {
                if (!_targetUI.TryAdd(targetKey, uiElement))
                    Debug.LogWarning("The targetKey already exists in the dictionary.");
            }

            if (uiElement is UIButton button)
            {
                _uiButtons.Add(button); 
            }
        }

        public UIElement GetTarget(string targetKey)
        {
            if (_targetUI.TryGetValue(targetKey, out UIElement uiElement))
            {
                return uiElement;
            }

            Debug.LogWarning("The targetKey does not exist in the dictionary.");
            return null;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var pos = Input.mousePosition;
                foreach (var item in _uiButtons)
                {
                    item.Down(pos);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var pos = Input.mousePosition;
                foreach (var item in _uiButtons)
                {
                    item.Up(pos);
                }
            } 
        }
    }
}