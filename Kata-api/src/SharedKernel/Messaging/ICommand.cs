namespace SharedKernel.Messaging;

/// <summary>
/// Marker interface for a command that performs a write operation and returns no value.
/// Implement as a record: <c>public record DeleteUserCommand(Guid UserId) : ICommand;</c>
/// </summary>
public interface ICommand;

/// <summary>
/// Marker interface for a command that performs a write operation and returns a value of type <typeparamref name="TResponse"/>.
/// Implement as a record: <c>public record CreateUserCommand(string Name) : ICommand&lt;Guid&gt;;</c>
/// </summary>
/// <typeparam name="TResponse">The type of the value returned on success.</typeparam>
public interface ICommand<TResponse>;
