namespace SharedKernel;

/// <summary>
/// Abstraction over the system clock for testable time-dependent code.
/// Inject this interface instead of using <see cref="DateTime.UtcNow"/> directly.
/// Use <c>TimeProvider.System</c> in production and a fake implementation in tests.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }
}
