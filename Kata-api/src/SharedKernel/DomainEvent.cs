namespace SharedKernel;

/// <summary>
/// Abstract base record for domain events. Automatically generates a time-ordered v7 GUID identifier
/// and sets the <see cref="OccurredOnUtc"/> timestamp. Inherit from this to define concrete domain events.
/// </summary>
/// <example>
/// <code>
/// public sealed record UserCreatedDomainEvent(Guid UserId) : DomainEvent;
/// </code>
/// </example>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
