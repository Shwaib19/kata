namespace SharedKernel;

/// <summary>
/// Marks an entity as soft-deletable. Instead of physical deletion, the entity is flagged as deleted.
/// Infrastructure (e.g. EF Core global query filters) should exclude soft-deleted entities from queries by default.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets a value indicating whether this entity has been soft-deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Gets the UTC timestamp of when this entity was deleted, or <c>null</c> if not deleted.
    /// </summary>
    DateTime? DeletedOnUtc { get; }

    /// <summary>
    /// Marks this entity as soft-deleted by setting <see cref="IsDeleted"/> and <see cref="DeletedOnUtc"/>.
    /// </summary>
    void Delete();
}
