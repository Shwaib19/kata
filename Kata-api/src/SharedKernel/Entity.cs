namespace SharedKernel;

/// <summary>
/// Base class for all domain entities. Provides identity, and domain event support.
/// Inherit from this class for any entity that participates in domain-driven design.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets a snapshot of the domain events raised by this entity.
    /// </summary>
    public List<IDomainEvent> DomainEvents => [.. _domainEvents];


    /// <summary>
    /// Clears all pending domain events. Typically called after events have been dispatched or persisted to the outbox.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Raises a domain event to be dispatched after the entity is persisted.
    /// </summary>
    protected void Raise(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }
}
