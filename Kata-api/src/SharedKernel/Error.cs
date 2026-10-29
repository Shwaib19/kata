namespace SharedKernel;

/// <summary>
/// Represents a domain error with a machine-readable code, human-readable description, and categorized type.
/// Use the static factory methods to create errors of specific types.
/// </summary>
public record Error
{
    /// <summary>
    /// Represents the absence of an error. Used internally by <see cref="Result"/> for success states.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    /// <summary>
    /// A general-purpose error indicating a null value was provided where one was not expected.
    /// </summary>
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);

    /// <summary>
    /// Creates an error indicating an invalid value was provided.
    /// </summary>
    /// <param name="description">A description of what made the value invalid, or <c>null</c> for a default message.</param>
    public static Error InvalidValue(string? description) => new(
        "General.InvalidValue",
        description ?? "An invalid value was provided.",
        ErrorType.Failure);

    /// <summary>
    /// Initializes a new <see cref="Error"/> instance.
    /// </summary>
    /// <param name="code">A machine-readable error code (e.g. <c>"User.NotFound"</c>).</param>
    /// <param name="description">A human-readable description of the error.</param>
    /// <param name="type">The category of error.</param>
    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    /// <summary>
    /// Gets the machine-readable error code (e.g. <c>"User.NotFound"</c>).
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable description of the error.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the category of this error, used to map to HTTP status codes.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Creates a general failure error (maps to HTTP 500).
    /// </summary>
    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    /// <summary>
    /// Creates a not-found error (maps to HTTP 404).
    /// </summary>
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    /// <summary>
    /// Creates a problem error for unexpected server-side issues (maps to HTTP 500).
    /// </summary>
    public static Error Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);

    /// <summary>
    /// Creates a conflict error when an operation conflicts with the current state (maps to HTTP 409).
    /// </summary>
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    /// <summary>
    /// Creates a validation/bad-request error (maps to HTTP 422).
    /// </summary>
    public static Error BadRequest(string code, string description) =>
        new(code, description, ErrorType.Validation);

    /// <summary>
    /// Creates a rate-limit error (maps to HTTP 429).
    /// </summary>
    public static Error TooManyRequests(string code, string description) =>
        new(code, description, ErrorType.TooManyRequests);

    /// <summary>
    /// Creates a generic unexpected error with an auto-generated code (maps to HTTP 500).
    /// </summary>
    /// <param name="description">Optional description, defaults to a generic message.</param>
    public static Error Unexpected(string? description) =>
        new("Error.Unexpected", description ?? "An unexpected error occurred.", ErrorType.Problem);

    /// <summary>
    /// Creates a forbidden error when the caller lacks permission (maps to HTTP 403).
    /// </summary>
    /// <param name="code">A machine-readable error code.</param>
    /// <param name="message">Optional message, defaults to a generic forbidden message.</param>
    public static Error Forbidden(string code, string? message) =>
        new(code, message ?? "You do not have permission to access this resource.", ErrorType.Forbidden);

    /// <summary>
    /// Creates a service-unavailable error for downstream dependency failures (maps to HTTP 503).
    /// </summary>
    public static Error ServiceUnavailable(string code, string description) =>
        new(code, description, ErrorType.ServiceUnavailable);
}
