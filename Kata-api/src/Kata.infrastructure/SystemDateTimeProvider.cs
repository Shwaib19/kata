using SharedKernel;

namespace Kata.infrastructure;

/// <summary>
/// The real clock. Tests substitute their own <see cref="IDateTimeProvider"/> so that
/// time-dependent rules, such as reservation expiry, stay controllable.
/// </summary>
internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
