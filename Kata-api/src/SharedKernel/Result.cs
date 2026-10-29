using System.Diagnostics.CodeAnalysis;

namespace SharedKernel;

/// <summary>
/// Represents the outcome of an operation that can succeed or fail.
/// Use <see cref="Success()"/> and <see cref="Failure(Error)"/> factory methods to create instances.
/// Prefer this over throwing exceptions for expected business failures.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new <see cref="Result"/> instance. Validates that success/error states are consistent.
    /// </summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="error">The error (must be <see cref="Error.None"/> for success, non-None for failure).</param>
    /// <exception cref="ArgumentException">Thrown when success/error state is inconsistent.</exception>
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error associated with the failure. Returns <see cref="Error.None"/> for success results.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Creates a successful result with no value.
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The success value.</param>
    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    /// <summary>
    /// Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error">The error describing why the operation failed.</param>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a typed failure result with the specified error and no value.
    /// </summary>
    /// <typeparam name="TValue">The expected value type.</typeparam>
    /// <param name="error">The error describing why the operation failed.</param>
    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);


    /// <summary>
    /// Creates a new <see cref="Result{TValue}"/> with the specified nullable value.
    /// Returns <see cref="Error.NullValue"/> when the value is null.
    /// </summary>
    /// <typeparam name="TValue">The result type.</typeparam>
    /// <param name="value">The result value.</param>
    /// <returns>A success result when value is not null; otherwise a failure with <see cref="Error.NullValue"/>.</returns>
    public static Result<TValue> Create<TValue>(TValue value)
    {
        return value is null
            ? Failure<TValue>(Error.NullValue)
            : Success(value);
    }

    /// <summary>
    /// Creates a new <see cref="Result{TValue}"/> with the specified nullable value and the specified error.
    /// </summary>
    /// <typeparam name="TValue">The result type.</typeparam>
    /// <param name="value">The result value.</param>
    /// <param name="error">The error in case the value is null.</param>
    /// <returns>A new instance of <see cref="Result{TValue}"/> with the specified value or an error.</returns>
    public static Result<TValue> Create<TValue>(TValue value, Error error)
    {
        if (value is null)
        {
            return Failure<TValue>(error);
        }

        return Success(value);
    }
}

/// <summary>
/// Represents the outcome of an operation that can succeed with a value of type <typeparamref name="TValue"/> or fail with an <see cref="Error"/>.
/// Supports implicit conversion from <typeparamref name="TValue"/> (non-null produces success, null produces failure).
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
public sealed class Result<TValue> : Result
{
    /// <summary>
    /// Initializes a new <see cref="Result{TValue}"/> instance.
    /// </summary>
    /// <param name="value">The result value (can be <c>null</c> for failure results).</param>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="error">The associated error.</param>
    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value!;
    }

    /// <summary>
    /// Gets the success value. Throws <see cref="InvalidOperationException"/> if the result is a failure.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessing the value of a failure result.</exception>
    [NotNull]
    public TValue Value
    {
        get => IsSuccess
            ? field!
            : throw new InvalidOperationException("The value of a failure result can't be accessed.");
        private init;
    }

    /// <summary>
    /// Implicitly converts a value to a <see cref="Result{TValue}"/>.
    /// Non-null values produce a success result; null produces a failure with <see cref="Error.NullValue"/>.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
