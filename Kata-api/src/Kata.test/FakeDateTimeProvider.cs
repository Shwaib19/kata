using SharedKernel;

namespace Kata.test;

internal sealed class FakeDateTimeProvider(DateTime utcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; } = utcNow;
}
