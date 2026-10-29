namespace SharedKernel.Outbox;

public sealed class InboxMessage
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string EventType { get; init; }
    public required string Content { get; init; }
    public required string SourceModule { get; init; }
    public DateTime ReceivedOnUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryAfterUtc { get; set; }
}
