using System;
using System.Collections.Generic;
using UnityEngine;
internal static class ListPool<T>
{
    // Object pool to avoid allocations.
    private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, Clear);
    static void Clear(List<T> l) { l.Clear(); }

    public static List<T> Get()
    {
        return s_ListPool.Get();
    }

    public static void Release(List<T> toRelease)
    {
        s_ListPool.Release(toRelease);
    }
}
/************
 * 
 * List<Vector3> m_Positions = ListPool<Vector3>.Get();
 * ListPool<Vector3>.Release(m_Positions);
 * 创建和销毁一定要成对出现
 ***********/
