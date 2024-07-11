using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ButtonAttribute : PropertyAttribute
{
    public string MethodName { get; private set; }
    public Type TargetType { get; private set; }
    public string ButtonName { get; private set; }

    public ButtonAttribute(string methodName, Type targetType, string buttonName = null)
    {
        MethodName = methodName;
        TargetType = targetType;
        ButtonName = buttonName;
    }
}