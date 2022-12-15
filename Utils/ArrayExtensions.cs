using System.Collections.Generic;
using System.Security.Cryptography;

namespace FiveWords.Utils;

public static class ArrayExtensions
{
    public static IEnumerable<T> GetElementsAt<T>(this T[] array, IEnumerable<int> indices)
    {
        var indicesEnumerator = indices.GetEnumerator();
        while (indicesEnumerator.MoveNext())
            yield return array[indicesEnumerator.Current];
        indicesEnumerator.Dispose();
    }

    // Не делаются никакие проверки, в расчёте на то, что все вызовы будут корректными и на корректных данных
    public static IEnumerable<T> ShuffleAndTake<T>(this T[] array, int count, Predicate<T> filter)
    {
        HashSet<int> usedIndices = new HashSet<int>(count);
        int returnedEntriesCount = 0;
        while (returnedEntriesCount < count)
        {
            var randomIndex = RandomNumberGenerator.GetInt32(array.Length - usedIndices.Count);
            randomIndex += usedIndices.Count(it => it < randomIndex);
            usedIndices.Add(randomIndex);
            if (filter(array[randomIndex]))
            {
                yield return array[randomIndex];
                returnedEntriesCount++;
            }
        }
    }
}
