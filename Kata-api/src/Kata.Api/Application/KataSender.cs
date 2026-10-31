using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Application;

/// <summary>
/// Resolves the handler registered for a command or query and invokes it.
/// </summary>
internal sealed class KataSender(IServiceProvider serviceProvider) : ISender
{
    public Task<Result<TResponse>> SendAsync<TResponse>(
        IQuery<TResponse> query, CancellationToken cancellationToken = default) =>
        Invoke<Task<Result<TResponse>>>(
            typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse)),
            query,
            cancellationToken);

    public Task<Result<TResponse>> SendAsync<TResponse>(
        ICommand<TResponse> command, CancellationToken cancellationToken = default) =>
        Invoke<Task<Result<TResponse>>>(
            typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse)),
            command,
            cancellationToken);

    public Task<Result> SendAsync<TResponse>(ICommand command, CancellationToken cancellationToken = default) =>
        SendAsync(command, cancellationToken);

    public Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default) =>
        Invoke<Task<Result>>(
            typeof(ICommandHandler<>).MakeGenericType(command.GetType()),
            command,
            cancellationToken);

    private TResult Invoke<TResult>(Type handlerType, object message, CancellationToken cancellationToken)
    {
        object handler = serviceProvider.GetRequiredService(handlerType);

        return (TResult)handlerType.GetMethod("Handle")!.Invoke(handler, [message, cancellationToken])!;
    }
}
