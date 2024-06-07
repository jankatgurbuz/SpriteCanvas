using SC.Core.UI;
using UnityEngine;

namespace SC.DemoGame.Tests
{
    [ExecuteInEditMode]
    public class SpriteCanvasTest : MonoBehaviour
    {
        public bool IsActive = false;

        private void Update()
        {
            if (IsActive)
            {
                GetComponent<SpriteCanvas>().ShowAllUIs();
            }
            else
            {
                GetComponent<SpriteCanvas>().HideAllUIs();
            }
           
        }
    }
}
