using SC.Core.UI;
using TMPro;
using UnityEngine;

namespace Samples.Demo.Scripts.DemoGame
{
    public class DemoGameNavigatorItem : MonoBehaviour
    {
        [SerializeField] private DemoGameButtonClickForGroup _button;
        [SerializeField] private TextMeshPro _tmpProText;
        [SerializeField] private UIElement _uiElement;
        [SerializeField] private Transform _parent;
        private Vector3 _newScale;
        private Vector3 _localPosition;
        private Vector3 _tempPos;

        private void Awake()
        {
            _uiElement.transform.SetParent(_parent.parent);
            _newScale = _uiElement.transform.localScale * 1.15f;
            _tempPos = _uiElement.transform.position +
                       (_uiElement.transform.position - _uiElement.transform.up * -1).normalized * 0.50f;
            _uiElement.transform.SetParent(_parent);
        }

        public void OnItemSelectionChanged(int index)
        {
            if (_button.ButtonIndex == index)
            {
                _tmpProText.gameObject.SetActive(true);

                _uiElement.UIElementProperties.IgnoreYPosition = true;
                _uiElement.UIElementProperties.IgnoreYScale = true;
                _uiElement.UIElementProperties.IgnoreXScale = true;

                _uiElement.transform.SetParent(_parent.parent);
                _uiElement.transform.localScale = _newScale;
                _uiElement.transform.position = _tempPos;
            }
            else
            {
                _uiElement.transform.SetParent(_parent);
                _tmpProText.gameObject.SetActive(false);

                _uiElement.UIElementProperties.IgnoreYPosition = false;
                _uiElement.UIElementProperties.IgnoreYScale = false;
                _uiElement.UIElementProperties.IgnoreXScale = false;
            }
        }
    }
}