using System;
using UnityEngine;
namespace SC.DemoGame
{
    public class DemoButtonClick : MonoBehaviour
    {
        public static DemoButtonClick Instance;
        private void Awake()
        {
            Instance = this;
        }

        public event Action OnClick;
        public void Down()
        {
           
        }

        public void Click()
        {
            OnClick?.Invoke();
        }
    }
}
