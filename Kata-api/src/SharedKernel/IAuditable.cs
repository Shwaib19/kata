namespace SharedKernel;

/// <summary>
/// Marks an entity as auditable by tracking creation and last-modification timestamps.
/// Implement on entities where you need automatic audit trail support.
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Gets the UTC timestamp of when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the UTC timestamp of when the entity was last updated, or <c>null</c> if never modified.
    /// </summary>
    DateTime? UpdatedAt { get; }
}
