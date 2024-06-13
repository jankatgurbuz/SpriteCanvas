using System;
using System.Reflection;
using SC.Core.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SC.Editor.Helpers
{
    public static class SpriteCanvasUpdater
    {
        private const string CameraViewportPropertiesName = "UpdateCameraViewportProperties";
        private const string RegisterTypeFieldName = "_registerType";
        private const string SpriteCanvasFieldName = "_spriteCanvas";
        private const string CanvasKey = "_canvasKey";
        
        public static void OnEditorUpdate()
        {
            if (Application.isPlaying)
                return;

            var objects = Object.FindObjectsOfType<SpriteCanvas>();

            foreach (var item in objects)
            {
                Handle(item);
            }
        }

        private static void Handle(SpriteCanvas currentSpriteCanvas)
        {
            InvokeMethod<SpriteCanvas>(currentSpriteCanvas, CameraViewportPropertiesName);

            var objects = Object.FindObjectsOfType<UIElement>();
            foreach (var item in objects)
            {
                var value = (UIElement.RegisterType)GetValue<UIElement>(item, RegisterTypeFieldName);

                if (value == UIElement.RegisterType.Hierarchy)
                {
                    SpriteCanvas sc = null;
                    var currentParent = item.transform.parent;

                    while (currentParent != null)
                    {
                        if (currentParent.TryGetComponent(out SpriteCanvas spriteCanvas))
                            sc = spriteCanvas;

                        currentParent = currentParent.parent;
                    }

                    if (sc == null || sc != currentSpriteCanvas) continue;
                }
                else if (value == UIElement.RegisterType.Reference)
                {
                    var reference = (SpriteCanvas)GetValue<UIElement>(item, SpriteCanvasFieldName);
                    if (reference == null || reference != currentSpriteCanvas) continue;
                }
                else if (value == UIElement.RegisterType.Key)
                {
                    if (item.CanvasKey != (string)GetValue<SpriteCanvas>(currentSpriteCanvas, CanvasKey)) continue;
                }

                item.ArrangeLayers(currentSpriteCanvas.SortingLayerName, currentSpriteCanvas.SortingLayerOrder);
                item.SetUILayout(currentSpriteCanvas.ViewportHeight, currentSpriteCanvas.ViewportWidth,
                    currentSpriteCanvas.ViewportPosition,
                    currentSpriteCanvas.Balance);
                item.SetUIElementProperties(currentSpriteCanvas.ElementProperties);
            }
        }

        private static FieldInfo GetField<T>(string fieldName, Type type)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance |
                                       BindingFlags.FlattenHierarchy;
            var field = typeof(T).GetField(fieldName, flags) ?? type.GetField(fieldName, flags);
            return field;
        }

        private static object GetValue<T>(object obj, string fieldName)
        {
            var info = GetField<T>(fieldName, obj.GetType());
            return info?.GetValue(obj);
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