using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

[Serializable]
public class InspectorTogglable<T>
    where T : struct
{
    [SerializeField, OdinSerialize] public bool IsEnabled;

    [SerializeField, OdinSerialize, EnableIf("IsEnabled"), HideLabel] public T Value;

    public InspectorTogglable(T value, bool isEnabled = false)
    {
        IsEnabled = isEnabled;
        Value = value;
    }
}
