namespace SharedKernel;

/// <summary>
/// Categorizes errors to enable mapping to appropriate HTTP status codes or response handling.
/// </summary>
public enum ErrorType
{
    /// <summary>General failure (HTTP 500).</summary>
    Failure = 0,
    /// <summary>Input validation failure (HTTP 422).</summary>
    Validation = 1,
    /// <summary>Unexpected server-side problem (HTTP 500).</summary>
    Problem = 2,
    /// <summary>Requested resource was not found (HTTP 404).</summary>
    NotFound = 3,
    /// <summary>Operation conflicts with current state (HTTP 409).</summary>
    Conflict = 4,
    /// <summary>Caller lacks permission (HTTP 403).</summary>
    Forbidden = 5,
    /// <summary>Rate limit exceeded (HTTP 429).</summary>
    TooManyRequests = 6,
    /// <summary>Downstream service unavailable (HTTP 503).</summary>
    ServiceUnavailable = 7,
    //TODO: badrequest and unauthorized could be added later
}
