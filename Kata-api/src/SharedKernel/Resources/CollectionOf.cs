namespace SharedKernel.Resources;

/// <summary>
/// A typed, immutable collection wrapper that pairs items with a total count.
/// Use for query results where the caller needs to know the collection size.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public sealed class CollectionOf<T>
{
    /// <summary>
    /// Gets the items in the collection.
    /// </summary>
    public IReadOnlyList<T> Items { get; private set; } = [];

    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    public int TotalCount { get; private set; }

    private CollectionOf()
    {
    }

    /// <summary>
    /// Creates a collection from the specified items. Total count is derived from the item count.
    /// </summary>
    /// <param name="items">The items to include.</param>
    public static CollectionOf<T> Create(IEnumerable<T> items)
    {
        var itemList = items.ToList();
        return new CollectionOf<T>
        {
            Items = itemList,
            TotalCount = itemList.Count
        };
    }

    /// <summary>
    /// Creates a collection from the specified items with an explicit total count
    /// (useful when items represent a page of a larger result set).
    /// </summary>
    /// <param name="items">The items to include.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    public static CollectionOf<T> Create(IEnumerable<T> items, int totalCount)
    {
        return new CollectionOf<T>
        {
            Items = [.. items],
            TotalCount = totalCount
        };
    }


    /// <summary>
    /// Gets an empty collection with zero items and a total count of 0.
    /// Use this to return an empty result set instead of null, following the Null Object Pattern.
    /// </summary>
    /// <returns>An empty <see cref="CollectionOf{T}"/> instance with no items and TotalCount = 0.</returns>
    public static CollectionOf<T> Empty() => new();
}

/// <summary>
/// Non-generic helper for creating <see cref="CollectionOf{T}"/> instances with type inference.
/// </summary>
public static class CollectionOf
{
    /// <summary>
    /// Creates an empty collection of the specified type with zero items and a total count of 0.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <returns>An empty <see cref="CollectionOf{T}"/> instance.</returns>
    public static CollectionOf<T> Empty<T>() => CollectionOf<T>.Empty();

    /// <summary>
    /// Creates a collection from the specified items with type inference.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The items to include.</param>
    public static CollectionOf<T> Create<T>(IEnumerable<T> items)
    {
        return CollectionOf<T>.Create(items);
    }
}
