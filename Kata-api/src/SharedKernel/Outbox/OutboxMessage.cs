namespace SharedKernel.Outbox;

public sealed class OutboxMessage : IOutboxMessage
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string EventType { get; init; }
    public required string Content { get; init; }
    public DateTime CreatedOnUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryAfterUtc { get; set; }

    public static OutboxMessage Create(string type, string content)
    {
        return new OutboxMessage
        {
            Id = Guid.CreateVersion7(),
            EventType = type,
            Content = content,
            CreatedOnUtc = DateTime.UtcNow,
            RetryCount = 0
        };
    }
}
