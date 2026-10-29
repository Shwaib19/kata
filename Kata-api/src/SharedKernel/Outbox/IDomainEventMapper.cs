namespace SharedKernel.Outbox;

/// <summary>
/// Converts a domain event stored in a module's outbox into an integration event
/// for cross-module delivery via the inbox pattern.
///
/// Implementations live in the <b>emitting module</b> (e.g. Kargo.Auth) and are registered
/// with the DI container. The <c>OutboxDispatcher</c> resolves all mappers and matches
/// them against each outbox row by <see cref="DomainEventTypeName"/>.
///
/// RULE: Only domain events (raised inside aggregates) flow through the outbox.
///       Integration events (cross-module contracts) are produced here, never inside domain entities.
/// </summary>
public interface IDomainEventMapper
{
    /// <summary>
    /// The <see cref="System.Type.FullName"/> of the domain event type this mapper handles.
    /// Matched against <see cref="OutboxMessage.EventType"/> (which also stores FullName).
    /// </summary>
    string DomainEventTypeName { get; }

    /// <summary>
    /// Maps the serialised domain event JSON to an integration event.
    /// </summary>
    /// <returns>
    /// The <see cref="System.Type.AssemblyQualifiedName"/> of the produced integration event type
    /// and its JSON-serialised content.
    /// </returns>
    (string IntegrationEventType, string IntegrationEventContent) Map(string domainEventJson);
}

