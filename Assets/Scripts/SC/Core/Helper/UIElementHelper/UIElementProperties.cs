using SC.Core.SpriteCanvasAttribute;
using SC.Core.Utility;
using UnityEngine;

namespace SC.Core.Helper.UIElementHelper
{
    [System.Serializable]
    public class UIElementProperties
    {
        [SerializeField] private int _orderInLayer;

        [SerializeField, HorizontalLine(EColor.Black, 1, 1)]
        private bool _ignoreXPosition;

        [SerializeField] private bool _ignoreYPosition;

        [SerializeField] private bool _ignoreXScale;

        [SerializeField] private bool _ignoreYScale;

        [SerializeField] private bool _useInitialCameraPosition;

        [SerializeField, HorizontalLine(EColor.Black, 1, 1)]
        private string _targetKey;

        public bool IgnoreXPosition
        {
            get => _ignoreXPosition;
            set => _ignoreXPosition = value;
        }

        public bool IgnoreYPosition
        {
            get => _ignoreYPosition;
            set => _ignoreYPosition = value;
        }

        public bool IgnoreXScale
        {
            get => _ignoreXScale;
            set => _ignoreXScale = value;
        }

        public bool IgnoreYScale
        {
            get => _ignoreYScale;
            set => _ignoreYScale = value;
        }

        public int OrderInLayer => _orderInLayer;

        public string TargetKey => _targetKey;

        public bool UseInitialCameraPosition => _useInitialCameraPosition;
    }
}