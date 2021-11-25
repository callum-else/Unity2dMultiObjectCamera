using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MinMax<T>
    where T : struct
{
    [HorizontalGroup("MinMax"), LabelWidth(50)]
    public T min;
    [HorizontalGroup("MinMax", marginLeft: 5), LabelWidth(50)]
    public T max;

    public MinMax(T minimum, T maximum)
    {
        min = minimum;
        max = maximum;
    }
}
