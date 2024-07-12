using SC.DemoGame;
using TMPro;
using UnityEngine;

namespace Samples.Demo.Scripts.DemoGame
{
    public class DemoGameCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _tmpText;
        private int _counter = 0;

        public void Start()
        {
            DemoGameManager.Instance.PopAction += Pop;
            DemoButtonClick.Instance.OnClick += Reset;
            _tmpText = GetComponent<TextMeshPro>();
            UpdateText();
        }

        private void Reset()
        {
            _counter = 0;
            UpdateText();
        }

        private void Pop()
        {
            _counter++;
            UpdateText();
        }

        private void UpdateText()
        {
            _tmpText.text = _counter.ToString();
        }
    }
}