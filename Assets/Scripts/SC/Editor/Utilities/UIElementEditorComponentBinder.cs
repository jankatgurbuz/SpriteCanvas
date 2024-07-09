using SC.Core.UI;
using TMPro;
using UnityEngine;

namespace SC.Editor.Utilities
{
    public partial class UIElementEditor
    {
        private const string ItemPositionFieldName = "_itemPosition";
        private const string SpriteRendererFieldName = "_spriteRenderer";
        private const string TextMeshProFieldName = "_textMeshPro";

        public void AssignComponent()
        {
            AssignComponent<Transform>(ItemPositionFieldName);
            AssignComponent<SpriteRenderer>(SpriteRendererFieldName);
            AssignComponent<TextMeshPro>(TextMeshProFieldName);
        }

        private void AssignComponent<T>(string fieldName) where T : Component
        {
            var currentValue = (T)GetValue<T>(target, fieldName);

            if (currentValue == null)
            {
                var component = ((UIElement)target).GetComponent<T>();
                SetValue<UIElement>(target, fieldName, component);
            }
        }
    }
}