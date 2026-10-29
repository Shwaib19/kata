namespace SharedKernel.Outbox;

/// <summary>
/// Port for writing into a module's inbox table. Each module registers its own implementation.
/// The <c>OutboxDispatcher</c> resolves all writers and routes converted integration events to
/// whichever writers declare interest via <see cref="Handles"/>.
/// </summary>
public interface IInboxWriter
{
    /// <summary>Name of the target module that owns this inbox (e.g. "Communication").</summary>
    string TargetModule { get; }

    /// <summary>
    /// Returns <c>true</c> if this writer is interested in the given integration event type.
    /// <paramref name="integrationEventType"/> is an <see cref="System.Type.AssemblyQualifiedName"/>.
    /// </summary>
    bool Handles(string integrationEventType);

    Task WriteAsync(InboxMessage message, CancellationToken ct);
}

