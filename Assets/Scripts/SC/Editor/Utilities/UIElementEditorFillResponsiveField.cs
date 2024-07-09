using System;
using System.Collections.Generic;
using SC.Core.ResponsiveOperations;
using SC.Core.UI;
using UnityEditor;
using System.Linq;

namespace SC.Editor.Utilities
{
    public partial class UIElementEditor
    {
        private const string ResponsiveOperationFieldName = "_responsiveOperation";
        private List<IResponsiveOperation> _implementingTypes;
        private string[] _typeNames;
        private bool _initFlag;

        private void InitializeResponsiveOperation()
        {
            _implementingTypes = GenerateTypeInstances<IResponsiveOperation>();
            _typeNames = ConvertTypeListToArray(_implementingTypes);
        }

        private void FillResponsiveField()
        {
            var data = GetValue<UIElement>(target, ResponsiveOperationFieldName);
            var currentIndex = FindTypeIndexInArray(_implementingTypes, (ResponsiveOperation)data);
        
            EditorGUI.BeginChangeCheck();
            var selectedIndex = EditorGUILayout.Popup("Anchor Presets", currentIndex, _typeNames);
            if (!_initFlag || EditorGUI.EndChangeCheck())
            {
                var spriteUIOperationHandler = _implementingTypes[selectedIndex];
        
                if (data == null || data.GetType() != spriteUIOperationHandler.GetType())
                {
                    SetValue<UIElement>(target, ResponsiveOperationFieldName, spriteUIOperationHandler);
                }
        
                EditorUtility.SetDirty(target);
                _initFlag = true;
            }
        }
        
        private static List<T> GenerateTypeInstances<T>()
        {
            var typeList = GetAssemblies();
            return typeList.Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Select(type => (T)Activator.CreateInstance(type))
                .ToList();
        }
        
        private static string[] ConvertTypeListToArray<T>(List<T> list)
        {
            return list.Select(type => type.GetType().ToString().Split('.').Last()).ToArray();
        }
        
        private static int FindTypeIndexInArray<T>(List<T> array, T field)
        {
            return field == null ? 0 : array.FindIndex(type => type.GetType() == field.GetType());
        }
        
        private static IEnumerable<Type> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()).ToList();
        }
    }
}