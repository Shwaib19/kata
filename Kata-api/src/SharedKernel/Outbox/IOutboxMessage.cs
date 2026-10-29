namespace SharedKernel.Outbox;

/// <summary>
/// Represents a message in the outbox pattern for reliable async communication.
/// Infrastructure adapters implement this to persist outbox records.
/// </summary>
public interface IOutboxMessage
{
    /// <summary>
    /// Unique identifier for this outbox message.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Fully qualified type name of the domain event.
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Serialized domain event payload (JSON).
    /// </summary>
    string Content { get; }

    /// <summary>
    /// UTC timestamp of when the outbox message was created.
    /// </summary>
    DateTime CreatedOnUtc { get; }

    /// <summary>
    /// UTC timestamp of when the message was successfully processed, or <c>null</c> if pending/failed.
    /// </summary>
    DateTime? ProcessedOnUtc { get; }

    /// <summary>
    /// Error details if processing failed.
    /// </summary>
    string? Error { get; }

    /// <summary>
    /// Number of processing attempts.
    /// </summary>
    int RetryCount { get; }
}
