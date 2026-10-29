using System.Linq.Expressions;

namespace SharedKernel.Specifications;

/// <summary>
/// Defines a reusable query specification for an entity type.
/// Encapsulates filtering criteria, eager-loading includes, ordering, tracking behavior, and pagination
/// in a single object that can be applied by a repository or data access layer.
/// </summary>
/// <typeparam name="TEntity">The entity type this specification applies to. Must inherit from <see cref="Entity"/>.</typeparam>
public interface ISpecification<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Gets the filter expression, or <c>null</c> if no filtering is applied.
    /// </summary>
    Expression<Func<TEntity, bool>>? Criteria { get; }

    /// <summary>
    /// Gets the list of navigation properties to eager-load via expression-based includes.
    /// </summary>
    List<Expression<Func<TEntity, object>>> Includes { get; }

    /// <summary>
    /// Gets the list of navigation properties to eager-load via string-based includes.
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Gets the ascending order expression, or <c>null</c> if no ascending ordering is applied.
    /// </summary>
    Expression<Func<TEntity, object>>? OrderByExpression { get; }

    /// <summary>
    /// Gets the descending order expression, or <c>null</c> if no descending ordering is applied.
    /// </summary>
    Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; }

    /// <summary>
    /// Gets a value indicating whether the query should be executed without change tracking.
    /// </summary>
    bool AsNoTracking { get; }

    /// <summary>
    /// Gets the maximum number of items to return (page size). Only used when <see cref="IsPagingEnabled"/> is <c>true</c>.
    /// </summary>
    int Take { get; }

    /// <summary>
    /// Gets the number of items to skip. Only used when <see cref="IsPagingEnabled"/> is <c>true</c>.
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// Gets a value indicating whether paging is enabled for this specification.
    /// </summary>
    bool IsPagingEnabled { get; }
}
