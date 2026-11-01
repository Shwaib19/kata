using Kata.Domain.Book;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.Update;

internal sealed class UpdateBookCommandHandler(
    IBookWriteRepository bookWriteRepository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateBookCommand>
{
    public async Task<Result> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        // Loaded through the write repository, which tracks the entity so that the change
        // the domain applies is the one SaveChanges persists.
        Domain.Book.Book? book = await bookWriteRepository.GetById(command.BookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound);
        }

        Result updated = book.Update(
            command.Title,
            command.Author,
            command.PublicationYear,
            dateTimeProvider);

        if (updated.IsFailure)
        {
            return updated;
        }

        await bookWriteRepository.SaveChanges(cancellationToken);

        return Result.Success();
    }
}
