namespace SharedKernel;

public interface IOutboxWriter
{
    Task WriteAsync(IReadOnlyList<IDomainEvent> domainEvents, CancellationToken ct = default);
}
