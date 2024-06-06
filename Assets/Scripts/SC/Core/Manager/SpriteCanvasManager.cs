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

        public void Register(string str, SpriteCanvas spriteCanvas)
        {
            _spriteCanvasMap ??= new Dictionary<string, SpriteCanvas>();

            if (!_spriteCanvasMap.TryAdd(str, spriteCanvas))
            {
                Debug.LogError($"Failed to register SpriteCanvas with key {str}. It might already exist in the map.");
            }
        }

        public SpriteCanvas Get(string key)
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

            if (targetKey != string.Empty)
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