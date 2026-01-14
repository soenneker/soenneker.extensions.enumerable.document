using Soenneker.Documents.Document.Abstract;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Soenneker.Extensions.Enumerable.Document;

/// <summary>
/// A collection of helpful IEnumerable Document extension methods
/// </summary>
public static class EnumerableDocumentsExtension
{
    /// <summary>
    /// Materializes a sequence of documents into a <see cref="List{T}"/> containing their <see cref="IDocument.Id"/> values.
    /// </summary>
    /// <typeparam name="T">The document type.</typeparam>
    /// <param name="value">The document sequence. If <c>null</c>, returns an empty list.</param>
    /// <returns>A list of document IDs.</returns>
    /// <remarks>
    /// This method avoids LINQ and attempts to preallocate capacity when the count can be determined cheaply
    /// (via <see cref="ICollection{T}"/>, <see cref="IReadOnlyCollection{T}"/>, or <see cref="System.Linq.Enumerable.TryGetNonEnumeratedCount{TSource}"/>).
    /// When an indexable list is available (<see cref="IList{T}"/> / <see cref="IReadOnlyList{T}"/>), it uses a for-loop to reduce enumerator overhead.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<string> ToIds<T>(this IEnumerable<T>? value) where T : IDocument
    {
        if (value is null)
            return [];

        // Fast path: indexable lists
        if (value is IList<T> list)
        {
            var result = new List<string>(list.Count);
            for (var i = 0; i < list.Count; i++)
                result.Add(list[i].Id);

            return result;
        }

        if (value is IReadOnlyList<T> roList)
        {
            var result = new List<string>(roList.Count);
            for (var i = 0; i < roList.Count; i++)
                result.Add(roList[i].Id);

            return result;
        }

        // Preallocate if we can get a count cheaply
        int capacity = 0;

        if (value is ICollection<T> collection)
        {
            capacity = collection.Count;
        }
        else if (value is IReadOnlyCollection<T> roCollection)
        {
            capacity = roCollection.Count;
        }
        else if (value.TryGetNonEnumeratedCount(out int count))
        {
            capacity = count;
        }

        var result2 = capacity > 0 ? new List<string>(capacity) : new List<string>();

        foreach (T doc in value)
            result2.Add(doc.Id);

        return result2;
    }

    /// <summary>
    /// Determines whether a sequence contains a document whose <see cref="IDocument.Id"/> equals <paramref name="id"/>.
    /// </summary>
    /// <typeparam name="T">The document type.</typeparam>
    /// <param name="entityEnumerable">The document sequence. If <c>null</c>, returns <c>false</c>.</param>
    /// <param name="id">The document ID to search for.</param>
    /// <returns><c>true</c> if a matching document is found; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Avoids LINQ and avoids double-enumeration. For indexable lists it uses a for-loop; for counted collections
    /// it short-circuits when the count is zero; otherwise it performs a single pass enumeration.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsId<T>(this IEnumerable<T>? entityEnumerable, string id) where T : IDocument
    {
        if (entityEnumerable is null)
            return false;

        if (entityEnumerable is IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Id == id)
                    return true;
            }

            return false;
        }

        if (entityEnumerable is IReadOnlyList<T> roList)
        {
            for (var i = 0; i < roList.Count; i++)
            {
                if (roList[i].Id == id)
                    return true;
            }

            return false;
        }

        if (entityEnumerable is ICollection<T> collection && collection.Count == 0)
            return false;

        if (entityEnumerable is IReadOnlyCollection<T> roCollection && roCollection.Count == 0)
            return false;

        foreach (T item in entityEnumerable)
        {
            if (item.Id == id)
                return true;
        }

        return false;
    }
}