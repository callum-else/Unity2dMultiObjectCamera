using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MinMax<T>
    where T : struct
{
    public T min;
    public T max;

    public MinMax(T minimum, T maximum)
    {
        min = minimum;
        max = maximum;
    }
}
