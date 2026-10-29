namespace SharedKernel.Messaging;

/// <summary>
/// Marker interface for a query that performs a read operation and returns a value of type <typeparamref name="TResponse"/>.
/// Implement as a record: <c>public record GetUserByIdQuery(Guid Id) : IQuery&lt;UserResponse&gt;;</c>
/// </summary>
/// <typeparam name="TResponse">The type of the value returned on success.</typeparam>
public interface IQuery<TResponse>;

