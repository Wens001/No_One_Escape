﻿using System.Text;
using UnityEngine;
public static class StringPool
{
    private static int MaxCount = 100;

    // Object pool to avoid allocations.
    private static readonly ObjectPool<StringBuilder> Pool = new ObjectPool<StringBuilder>(Clear, null);
    static void Clear(StringBuilder s)
    {
        if (Pool.countAll >= MaxCount)
        {
            Debug.LogError("Pool count reach to MaxCount.");
        }
        s.Remove(0, s.Length);
    }

    public static StringBuilder GetStringBuilder()
    {
        StringBuilder stringBuilder = Pool.Get();
        return stringBuilder;
    }

    public static void Release(StringBuilder toRelease)
    {
        if (Pool.countAll >= MaxCount)
        {
            Debug.LogError("Pool count reach to MaxCount.");
        }

        Pool.Release(toRelease);
    }

    public static string Concat(string s1, string s2)
    {
        StringBuilder stringBuilder = Pool.Get();
        stringBuilder.Append(s1);
        stringBuilder.Append(s2);
        string result = stringBuilder.ToString();
        Release(stringBuilder);
        return result;
    }

    public static string Concat(string s1, string s2, string s3)
    {
        StringBuilder stringBuilder = Pool.Get();
        stringBuilder.Append(s1);
        stringBuilder.Append(s2);
        stringBuilder.Append(s3);
        string result = stringBuilder.ToString();
        Release(stringBuilder);
        return result;
    }

    public static string Concat(string s1, string s2, string s3,string s4)
    {
        StringBuilder stringBuilder = Pool.Get();
        stringBuilder.Append(s1);
        stringBuilder.Append(s2);
        stringBuilder.Append(s3);
        stringBuilder.Append(s4);
        string result = stringBuilder.ToString();
        Release(stringBuilder);
        return result;
    }

    public static string Format(string src, params object[] args)
    {
        StringBuilder stringBuilder = Pool.Get();
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.AppendFormat(src, args);
        string result = stringBuilder.ToString();
        Release(stringBuilder);
        return result;
    }
}

/*****************
 * string a = StringPool.Concat("a"+"b");
 * 
 * 
 ****************/
