namespace SharedKernel.Outbox;

/// <summary>
/// Port for reading a module's outbox table. Each module registers its own implementation.
/// The <c>OutboxDispatcher</c> polls all registered readers to route domain events to target inboxes.
/// </summary>
public interface IOutboxReader
{
    /// <summary>Identifies the source module (e.g. "Auth").</summary>
    string ModuleName { get; }

    Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int batchSize, int maxRetries, CancellationToken ct);

    Task MarkProcessedAsync(Guid id, CancellationToken ct);

    Task MarkFailedAsync(Guid id, string error, CancellationToken ct);
}

