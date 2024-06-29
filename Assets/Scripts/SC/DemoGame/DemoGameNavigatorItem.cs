using System;
using SC.Core.Helper;
using SC.Core.UI;
using TMPro;
using UnityEngine;

namespace SC.DemoGame
{
    public class DemoGameNavigatorItem : MonoBehaviour
    {
        [SerializeField] private DemoGameButtonClickForGroup _button;
        [SerializeField] private TextMeshPro _tmpProText;
        [SerializeField] private UIElement _uiElement;
        [SerializeField] private Transform _parent;
        private Vector3 _newScale;
        private Vector3 _localPosition;

        private void Awake()
        {
            _uiElement.transform.SetParent(_parent.parent);
            _newScale = _uiElement.transform.localScale * 1.15f;
            _uiElement.transform.SetParent(_parent);
        }

        private void Start()
        {
            OnItemSelectionChanged(2);
        }

        public void OnItemSelectionChanged(int index)
        {
            if (_button.ButtonIndex == index)
            {
                _tmpProText.gameObject.SetActive(true);
                
                _uiElement.IgnoreYPosition = true;
                _uiElement.IgnoreYScale = true;
                _uiElement.IgnoreXScale = true;

                _uiElement.transform.SetParent(_parent.parent);
                _uiElement.transform.localScale = _newScale;
                _uiElement.transform.localPosition =
                    new Vector3(_uiElement.transform.localPosition.x, 0.015f, _uiElement.transform.localPosition.z);
            }
            else
            {
                _uiElement.transform.SetParent(_parent);
                _tmpProText.gameObject.SetActive(false);

                _uiElement.IgnoreYPosition = false;
                _uiElement.IgnoreYScale = false;
                _uiElement.IgnoreXScale = false;
            }
        }
    }
}