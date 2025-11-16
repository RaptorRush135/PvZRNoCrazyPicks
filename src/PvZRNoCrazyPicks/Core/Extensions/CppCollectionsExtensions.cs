namespace PvZRNoCrazyPicks.Extensions;

using CppCollections = Il2CppSystem.Collections.Generic;

internal static class CppCollectionsExtensions
{
    public static IEnumerable<T> AsEnumerable<T>(
        this CppCollections.List<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);

        return AsEnumerableIterator();

        IEnumerable<T> AsEnumerableIterator()
        {
            int initialCount = list.Count;

            for (int i = 0; i < list.Count; i++)
            {
                if (list.Count != initialCount)
                {
                    throw new InvalidOperationException(
                        "Collection was modified. " +
                        $"Initial count: {initialCount}, Current count: {list.Count}.");
                }

                yield return list[i];
            }
        }
    }
}
