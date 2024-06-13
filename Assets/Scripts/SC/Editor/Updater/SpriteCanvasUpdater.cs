using SC.Core.UI;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Updater
{
    [InitializeOnLoad]
    public class SpriteCanvasUpdater
    {
        static SpriteCanvasUpdater()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            if (Application.isPlaying)
                return;

            var objects = Object.FindObjectsOfType<SpriteCanvas>();

            foreach (var item in objects)
            {
                item.UpdateForEditor();
            }
        }
    }
}
