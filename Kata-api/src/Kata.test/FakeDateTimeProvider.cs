using SharedKernel;

namespace Kata.test;

internal sealed class FakeDateTimeProvider(DateTime utcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; set; } = utcNow;

    public void Advance(TimeSpan duration) => UtcNow = UtcNow.Add(duration);
}
