namespace SharedKernel.Outbox;

/// <summary>
/// Port for reading a module's inbox table. Each module registers its own implementation.
/// The <c>InboxProcessor</c> polls all registered readers to dispatch integration events to handlers.
/// </summary>
public interface IInboxReader
{
    /// <summary>Identifies the consumer module (e.g. "Communication").</summary>
    string ModuleName { get; }

    Task<IReadOnlyList<InboxMessage>> GetPendingAsync(int batchSize, int maxRetries, CancellationToken ct);

    Task MarkProcessedAsync(Guid id, CancellationToken ct);

    Task MarkFailedAsync(Guid id, string error, CancellationToken ct);
}

