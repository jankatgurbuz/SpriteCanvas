
using System.Collections.Generic;
using UnityEngine;
using Core;
using Core.UI;

namespace Manager
{
    public class SpriteCanvasManager : MonoBehaviour
    {
        public static SpriteCanvasManager Instance;
        private Dictionary<string, SpriteCanvasTest> _spriteCanvasMap;

        private void Awake()
        {
            Instance = this;
        }

        public void Register(string str,SpriteCanvasTest spriteCanvas)
        {
            _spriteCanvasMap ??= new Dictionary<string,SpriteCanvasTest>();

            if (!_spriteCanvasMap.TryAdd(str, spriteCanvas))
            {
                Debug.LogError($"Failed to register SpriteCanvas with key {str}. It might already exist in the map.");
            }
        }

        public SpriteCanvasTest Get(string key)
        {
            if (_spriteCanvasMap.TryGetValue(key, out var spriteCanvas))
            {
                return spriteCanvas;
            }

            Debug.LogError($"No SpriteCanvas found with key {key}.");
            return null;
        }
    }
}
