
using System.Collections.Generic;
using UnityEngine;
using Core.UI;

namespace Manager
{
    public class SpriteCanvasManager : MonoBehaviour
    {
        public static SpriteCanvasManager Instance;
        private Dictionary<string, SpriteCanvas> _spriteCanvasMap;

        private void Awake()
        {
            Instance = this;
        }

        public void Register(string str,SpriteCanvas spriteCanvas)
        {
            _spriteCanvasMap ??= new Dictionary<string,SpriteCanvas>();

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
    }
}
