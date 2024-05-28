using System.Collections.Generic;
using UnityEngine;
using Core.UI;

namespace Manager
{
    public class SpriteCanvasManager : MonoBehaviour
    {
        public static SpriteCanvasManager Instance;
        private Dictionary<string, SpriteCanvas> _spriteCanvasMap;
        private Dictionary<string, UIElement> _targetUI;

        private void Awake()
        {
            Instance = this;
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
            
            if (targetKey == string.Empty) return;
            if (_targetUI.TryAdd(targetKey, uiElement)) return;
            
            Debug.Log(targetKey);
            Debug.LogWarning("The targetKey already exists in the dictionary.");
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
    }
}