using System;
using System.Reflection;
using SC.Core.Helper;
using SC.Core.Helper.Groups;
using SC.Core.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SC.Editor.Helpers
{
    public static class SpriteCanvasUpdater
    {
        private const string CameraViewportPropertiesName = "UpdateCameraViewportProperties";

        public static void OnEditorUpdate()
        {
            if (Application.isPlaying)
                return;

            var objects = Object.FindObjectsOfType<SpriteCanvas>();

            foreach (var item in objects)
            {
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
            InvokeMethod<SpriteCanvas>(currentSpriteCanvas, CameraViewportPropertiesName);
            var objects = Object.FindObjectsOfType<UIElement>();
            foreach (var item in objects)
            {
                item.Initialize();
            }

            var canvases = Object.FindObjectsOfType<SpriteCanvas>();
            foreach (var item in canvases)
            {
                item.AdjustDependentUIElements();
            }
        }

        private static MethodInfo GetMethodInfo<T>(string name, Type type)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance |
                                       BindingFlags.FlattenHierarchy;

            var field = typeof(T).GetMethod(name, flags) ?? type.GetMethod(name, flags);
            return field;
        }

        private static void InvokeMethod<T>(object obj, string fieldName, params object[] parameters)
        {
            var info = GetMethodInfo<T>(fieldName, obj.GetType());
            info.Invoke(obj, parameters);
        }
    }
}