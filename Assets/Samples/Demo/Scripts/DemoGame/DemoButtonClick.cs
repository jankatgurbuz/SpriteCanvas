using System;
using UnityEngine;

namespace Samples.Demo.Scripts.DemoGame
{
    public class DemoButtonClick : MonoBehaviour
    {
        public static DemoButtonClick Instance;
        public event Action OnClick;

        private void Awake()
        {
            Instance = this;
        }

        public void Down()
        {
        }

        public void Click()
        {
            OnClick?.Invoke();
        }
    }
}