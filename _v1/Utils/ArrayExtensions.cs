namespace FiveWords._v1.Utils;

public static class ArrayExtensions
{
    public static IEnumerable<T> GetElementsAt<T>(this T[] array, IEnumerable<int> indices)
    {
        var indicesEnumerator = indices.GetEnumerator();
        while (indicesEnumerator.MoveNext())
            yield return array[indicesEnumerator.Current];
        indicesEnumerator.Dispose();
    }
}
