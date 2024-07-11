using SC.Core.UI;
using UnityEngine;

namespace SC.Core.Helper.Groups
{
    [System.Serializable]
    public class GroupElementProperty
    {
        [SerializeField] private UIElement _uiElement;
        [SerializeField] private float _scaleRatio = 1;
        [SerializeField] private float _spaceRatio = 0;

        public UIElement UIElement
        {
            get => _uiElement;
            set => _uiElement = value;
        }

        public float ScaleRatio
        {
            get => _scaleRatio;
            set => _scaleRatio = value;
        }

        public float SpaceRatio
        {
            get => _spaceRatio;
            set => _spaceRatio = value;
        }
    }
}