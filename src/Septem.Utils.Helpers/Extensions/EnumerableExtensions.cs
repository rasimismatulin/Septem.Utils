using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Septem.Utils.Helpers.Extensions;

public static class EnumerableExtensions
{

    public static string JoinStrings(this IEnumerable<string> source, string separator = "\n") =>
        string.Join(separator, source);


    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var obj in source)
            action?.Invoke(obj);
    }

    public static IEnumerable<T> ForEachDo<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var obj in source)
        {
            action?.Invoke(obj);
            yield return obj;
        }
    }

    public static IQueryable<T> ForEachDo<T>(this IQueryable<T> source, Action<T> action)
    {
        foreach (var obj in source)
        {
            action?.Invoke(obj);
        }
        return source;
    }

    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> source, Action<T> action)
    {
        await foreach (var obj in source)
            action?.Invoke(obj);
    }

    public static async IAsyncEnumerable<T> ForEachDoAsync<T>(this IAsyncEnumerable<T> source, Action<T> action)
    {
        await foreach (var obj in source)
        {
            action?.Invoke(obj);
            yield return obj;
        }
    }



    public static ChangeResult<TSource, TSource> CompareTo<TSource, TKey>(this IEnumerable<TSource> local, IEnumerable<TSource> remote, Func<TSource, TKey> keySelector)
    {
        var remoteKeyValues = remote.ToDictionary(keySelector);

        var deleted = new List<TSource>();
        var changed = new List<Tuple<TSource, TSource>>();
        var localKeys = new HashSet<TKey>();

        foreach (var localItem in local)
        {
            var localKey = keySelector(localItem);
            localKeys.Add(localKey);

            if (remoteKeyValues.TryGetValue(localKey, out var changeCandidate))
            {
                if (!changeCandidate.Equals(localItem))
                    changed.Add(new Tuple<TSource, TSource>(localItem, changeCandidate));
            }
            else
            {
                deleted.Add(localItem);
            }
        }
        var inserted = remoteKeyValues.Where(x => !localKeys.Contains(x.Key)).Select(x => x.Value).ToList();

        return new ChangeResult<TSource, TSource>(deleted, changed, inserted);
    }


    public static void CompareToEach<TSource, TKey>(this IEnumerable<TSource> local, IEnumerable<TSource> remote, Func<TSource, TKey> keySelector,
        Action<TSource> forInserted = null, Action<TSource> forDeleted = null, Action<TSource, TSource> forChanged = null)
    {
        var remoteKeyValues = remote.ToDictionary(keySelector);
        var localKeys = new HashSet<TKey>();

        foreach (var localItem in local)
        {
            var localKey = keySelector(localItem);
            localKeys.Add(localKey);

            if (remoteKeyValues.TryGetValue(localKey, out var changeCandidate))
            {
                if (!changeCandidate.Equals(localItem))
                    forChanged?.Invoke(localItem, changeCandidate);
            }
            else
            {
                forDeleted?.Invoke(localItem);
            }
        }

        remoteKeyValues.Where(x => !localKeys.Contains(x.Key)).ForEach(x => forInserted.Invoke(x.Value));
    }
}