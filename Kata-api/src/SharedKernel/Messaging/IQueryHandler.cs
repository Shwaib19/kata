namespace SharedKernel.Messaging;

/// <summary>
/// Defines a handler for a query that returns a value of type <typeparamref name="TResponse"/>.
/// Implement one handler per query type. Handlers are resolved by <see cref="ISender"/> at runtime.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Handles the specified query asynchronously.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Result{TResponse}"/> containing the query result or an error.</returns>
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
