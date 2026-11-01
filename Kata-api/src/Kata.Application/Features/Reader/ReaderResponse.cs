using Kata.Application.Features.Reservation;

namespace Kata.Application.Features.Reader;

public sealed record ReaderResponse(
    Guid Id,
    string Name,
    string Email,
    IReadOnlyCollection<ReservationResponse> CurrentReservations)
{
    public static ReaderResponse FromDomain(
        Domain.Reader.Reader reader,
        IEnumerable<ReservationDetails> currentReservations,
        DateTime utcNow) =>
        new(reader.Id,
            reader.Name,
            reader.Email,
            currentReservations
                .Select(details => ReservationResponse.FromDomain(details, utcNow))
                .ToArray());
}
