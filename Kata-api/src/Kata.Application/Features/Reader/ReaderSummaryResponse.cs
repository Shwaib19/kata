namespace Kata.Application.Features.Reader;

/// <summary>
/// A reader as a list shows one: without the reservations themselves, but with how many books
/// the reader is holding, which is what both the list and the dashboard need.
/// </summary>
public sealed record ReaderSummaryResponse(
    Guid Id,
    string Name,
    string Email,
    int ActiveReservationCount)
{
    public static ReaderSummaryResponse FromDomain(Domain.Reader.Reader reader, int activeReservationCount) =>
        new(reader.Id, reader.Name, reader.Email, activeReservationCount);
}
