using Soenneker.Documents.Document.Abstract;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Soenneker.Extensions.Enumerable.Document;

/// <summary>
/// A collection of helpful IEnumerable Document extension methods
/// </summary>
public static class EnumerableDocumentsExtension
{
    /// <summary>
    /// Projects a sequence of <typeparamref name="T"/> documents into a list of their IDs.
    /// </summary>
    /// <typeparam name="T">The type of document, implementing <see cref="IDocument"/>.</typeparam>
    /// <param name="value">The enumerable collection of documents.</param>
    /// <returns>A list of document IDs. Returns an empty list if <paramref name="value"/> is <c>null</c>.</returns>
    /// <remarks>
    /// This method is optimized for performance by:
    /// <list type="bullet">
    ///   <item><description>Preallocating list capacity when the collection size is known via <see cref="ICollection{T}"/>.</description></item>
    ///   <item><description>Using index-based iteration for <see cref="IList{T}"/> to minimize iterator overhead.</description></item>
    ///   <item><description>Avoiding LINQ and deferred execution, returning a fully materialized list of strings.</description></item>
    /// </list>
    /// </remarks>
    [Pure]
    public static List<string> ToIds<T>(this IEnumerable<T> value) where T : IDocument
    {
        switch (value)
        {
            case null:
                return [];

            case ICollection<T> collection:
            {
                var result = new List<string>(collection.Count);
                // Use indexer if available to avoid iterator allocation
                if (collection is IList<T> list)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        result.Add(list[i].Id);
                    }
                }
                else
                {
                    foreach (T doc in collection)
                    {
                        result.Add(doc.Id);
                    }
                }

                return result;
            }

            default:
            {
                // Avoid multiple enumerator allocations if possible
                var result = new List<string>();
                using IEnumerator<T> enumerator = value.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    result.Add(enumerator.Current.Id);
                }

                return result;
            }
        }
    }

    /// <summary>
    /// Determines whether the specified <paramref name="entityEnumerable"/> contains a document with the given <paramref name="id"/>.
    /// </summary>
    /// <typeparam name="T">The type of document in the collection, implementing <see cref="IDocument"/>.</typeparam>
    /// <param name="entityEnumerable">The enumerable collection of documents to search.</param>
    /// <param name="id">The document ID to search for.</param>
    /// <returns><c>true</c> if a document with the specified ID is found; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method is optimized for performance by avoiding LINQ and minimizing allocations. It handles common collection types
    /// such as <see cref="IList{T}"/> and <see cref="ICollection{T}"/> efficiently, and uses manual enumeration when necessary.
    /// </remarks>
    [Pure]
    public static bool ContainsId<T>(this IEnumerable<T> entityEnumerable, string id) where T : IDocument
    {
        if (entityEnumerable.IsNullOrEmpty())
            return false;

        switch (entityEnumerable)
        {
            case IList<T> list:
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i].Id == id)
                        return true;
                }

                break;

            case ICollection<T> collection:
                foreach (T item in collection)
                {
                    if (item.Id == id)
                        return true;
                }

                break;

            default:
            {
                using IEnumerator<T> enumerator = entityEnumerable.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Id == id)
                        return true;
                }

                break;
            }
        }

        return false;
    }
}