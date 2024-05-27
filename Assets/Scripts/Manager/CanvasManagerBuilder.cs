using Manager;
using UnityEngine;

namespace SpriteCanvas.Scripts.Manager
{
    public static class CanvasManagerBuilder 
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            var managerObj = new GameObject("SpriteCanvasManager");
            managerObj.AddComponent<SpriteCanvasManager>();
            Object.DontDestroyOnLoad(managerObj);
        }
    }
}
