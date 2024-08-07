using System;
using System.Reflection;
using SC.Core.Helper.Groups;
using SC.Core.UI;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SC.Editor.Helpers
{
    [InitializeOnLoad]
    public static class SpriteCanvasUpdater
    {
        private const string CameraViewportPropertiesName = "UpdateCameraViewportProperties";

        static SpriteCanvasUpdater()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            if (Application.isPlaying) return;


            var objects = Object.FindObjectsOfType<SpriteCanvas>();
            foreach (var item in objects)
            {
                var checkCamera = (bool)InvokeMethod<SpriteCanvas>(item, "CheckCamera");
                if (!checkCamera) return;

                Handle(item);
            }

            var horizontalGroup = Object.FindObjectsOfType<HorizontalGroup>();
            foreach (var item in horizontalGroup)
            {
                item.AdjustGroup();
            }
        }

        private static void Handle(SpriteCanvas currentSpriteCanvas)
        {
            var canvases = Object.FindObjectsOfType<SpriteCanvas>();
            var objects = Object.FindObjectsOfType<UIElement>();

            foreach (var item in canvases)
            {
                item.AdjustCameraMode();
            }

            InvokeMethod<SpriteCanvas>(currentSpriteCanvas, CameraViewportPropertiesName);

            foreach (var item in objects)
            {
                item.Initialize();
            }


            foreach (var item in canvases)
            {
                item.AdjustDependentUIElements();
            }
        }

        private static MethodInfo GetMethodInfo<T>(string name, Type type)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            var field = typeof(T).GetMethod(name, flags) ?? type.GetMethod(name, flags);
            return field;
        }

        private static object InvokeMethod<T>(object obj, string fieldName, params object[] parameters)
        {
            var info = GetMethodInfo<T>(fieldName, obj.GetType());
            return info.Invoke(obj, parameters);
        }
    }
}