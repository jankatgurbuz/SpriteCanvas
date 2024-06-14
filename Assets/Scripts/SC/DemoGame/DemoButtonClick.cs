using System;
using UnityEngine;
using Action = Unity.Plastic.Antlr3.Runtime.Misc.Action;

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
