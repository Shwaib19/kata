namespace SharedKernel.Messaging;

/// <summary>
/// Dispatches commands and queries to their registered handlers.
/// The consuming project must provide an implementation (e.g. using <c>IServiceProvider</c> to resolve handlers at runtime).
/// </summary>
public interface ISender
{
    /// <summary>
    /// Dispatches a query to its handler and returns the result.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Result{TResponse}"/> containing the query result or an error.</returns>
    Task<Result<TResponse>>
        SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a command that returns a value to its handler.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Result{TResponse}"/> containing the command result or an error.</returns>
    Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a command that returns no value to its handler (generic overload).
    /// </summary>
    /// <typeparam name="TResponse">Unused type parameter for overload resolution.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> SendAsync<TResponse>(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a command that returns no value to its handler.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);
}
