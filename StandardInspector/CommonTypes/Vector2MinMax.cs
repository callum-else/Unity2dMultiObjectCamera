using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Vector2MinMax<T> 
    where T : struct
{
    public MinMax<T> x;
    public MinMax<T> y;

    public Vector2MinMax(MinMax<T> x, MinMax<T> y)
    {
        this.x = x;
        this.y = y;
    }
}
