using Core.UI;
using UnityEditor;
using UnityEngine;

namespace Editor.Updater
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

            var objects = Object.FindObjectsOfType<SpriteCanvasTest>();

            foreach (var item in objects)
            {
                item.UpdateForEditor();
            }
        }
    }
}
