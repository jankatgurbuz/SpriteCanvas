using UnityEngine;

namespace SC.Core.Helper
{
    [System.Serializable]
    public class UIElementSettings
    {
        [Range(0f, 1f)] public float Alpha = 1;
        public bool Interactable = true;
    }
}