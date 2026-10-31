using Kata.Application.Features.Book;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.Create;

internal sealed class CreateBookCommandHandler(
    IBookWriteRepository bookWriteRepository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateBookCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        // The domain owns every rule; the handler only orchestrates persistence.
        Result<Domain.Book.Book> book = Domain.Book.Book.Create(
            command.Title,
            command.Author,
            command.PublicationYear,
            dateTimeProvider);

        if (book.IsFailure)
        {
            return Result.Failure<Guid>(book.Error);
        }

        await bookWriteRepository.Add(book.Value, cancellationToken);
        await bookWriteRepository.SaveChanges(cancellationToken);

        return Result.Success(book.Value.Id);
    }
}
