using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SC.DemoGame
{
    public class DemoGameBeads : MonoBehaviour
    {
        [SerializeField] private DemoGameBeadsProp[] _demoGameBeadsProp;
        private Dictionary<DemoGameBeadsColor, GameObject> _objects;
        private Dictionary<DemoGameBeadsColor, GameObject> Objects => _objects;

        private void Awake()
        {
            _objects = new Dictionary<DemoGameBeadsColor, GameObject>();
            foreach (var item in _demoGameBeadsProp)
            {
                var obj = Create(item);
                _objects.Add(item.Key, obj);
            }
        }

        private GameObject Create(DemoGameBeadsProp prop)
        {
            var obj = new GameObject(prop.Sprite.name);
            var spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = prop.Sprite;
            obj.SetActive(false);
            return obj;
        }

        public (GameObject, DemoGameBeadsColor) GetRandomObject()
        {
            var values = new List<DemoGameBeadsColor>(_objects.Keys);
            var randomIndex = Random.Range(0, values.Count);
            return (_objects[values[randomIndex]], values[randomIndex]);
        }

        [System.Serializable]
        public class DemoGameBeadsProp
        {
            public DemoGameBeadsColor Key;
            public Sprite Sprite;
        }

        public enum DemoGameBeadsColor
        {
            Blue,
            Green,
            Orange,
            Purple,
            Red,
            Yellow,
            Empty
        }
    }
}