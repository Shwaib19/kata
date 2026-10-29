namespace SharedKernel;

/// <summary>
/// Represents a validation failure containing multiple field-level errors.
/// This is a single failure with multiple validation causes.
/// </summary>
public sealed record ValidationError : Error
{
    /// <summary>
    /// Initializes a new <see cref="ValidationError"/> with the specified field-level errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors. Must contain at least one error.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is null or empty.</exception>
    public ValidationError(IReadOnlyCollection<Error> errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        if (errors is null || errors.Count == 0)
        {
            throw new ArgumentException(
                "ValidationError must contain at least one error.",
                nameof(errors));
        }

        Errors = errors;
    }

    /// <summary>
    /// Gets the individual field-level validation errors.
    /// </summary>
    public IReadOnlyCollection<Error> Errors { get; }

    /// <summary>
    /// Creates a <see cref="ValidationError"/> from a collection of results, extracting errors from failed results only.
    /// </summary>
    /// <param name="results">The results to extract errors from.</param>
    /// <returns>A <see cref="ValidationError"/> containing all failure errors.</returns>
    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
