﻿namespace MikyM.Utilities.Extensions;

/// <summary>
/// Helper methods for the lists.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="chunkSize"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }
}
