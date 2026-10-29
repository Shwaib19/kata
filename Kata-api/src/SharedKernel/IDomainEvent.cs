namespace SharedKernel;

/// <summary>
/// Represents a domain event that occurred within an aggregate. Implement this interface
/// on event records to enable outbox-based event processing and cross-aggregate coordination.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of this event instance.
    /// </summary>
    Guid Id { get; init; }

    /// <summary>
    /// Gets the UTC timestamp of when this event occurred.
    /// </summary>
    DateTime OccurredOnUtc { get; init; }
}
