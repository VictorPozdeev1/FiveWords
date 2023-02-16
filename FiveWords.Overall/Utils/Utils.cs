using System.Security.Cryptography;

namespace FiveWords.Utils;

public static class Utils
{
    public static List<int> GetDifferentRandomIndices(byte amount, int upperLimiter/*, Predicate<int>? filter = null*/)
    {
        List<int> result = new();
        while (result.Count < amount)
        {
            var testIndex = RandomNumberGenerator.GetInt32(upperLimiter);
            //if (filter != null && filter(testIndex))
            if (!result.Contains(testIndex))
                result.Add(testIndex);
        }
        return result;
    }
}
