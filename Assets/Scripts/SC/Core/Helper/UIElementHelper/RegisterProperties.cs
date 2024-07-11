using SC.Core.SpriteCanvasAttribute;
using SC.Core.UI;
using UnityEngine;

namespace SC.Core.Helper.UIElementHelper
{
    [System.Serializable]
    public class RegisterProperties
    {
        [SerializeField] private RegisterType _registerType;

        [SerializeField, ConditionalVisibility("_register._registerType", RegisterType.Key), CanvasKeyValidator]
        private string _canvasKey;

        [SerializeField, ConditionalVisibility("_register._registerType", RegisterType.Reference)]
        private SpriteCanvas _spriteCanvas;

        public SpriteCanvas SpriteCanvas
        {
            get => _spriteCanvas;
            set => _spriteCanvas = value;
        }

        public RegisterType RegisterType => _registerType;
        public string CanvasKey => _canvasKey;
    }
}