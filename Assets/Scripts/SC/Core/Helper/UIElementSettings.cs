using UnityEngine;

namespace SC.Core.Helper
{
    [System.Serializable]
    public class UIElementSettings
    {
        [SerializeField, Range(0f, 1f)] private float _alpha = 1;

        [SerializeField] private bool _interactable = true;

        public float Alpha
        {
            get => _alpha;
            set => _alpha = value;
        }

        public bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }
    }
}