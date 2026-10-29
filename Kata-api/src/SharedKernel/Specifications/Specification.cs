using System.Linq.Expressions;

namespace SharedKernel.Specifications;

/// <summary>
/// Base class for building reusable, composable query specifications.
/// Inherit from this class and use the protected helper methods to configure filtering, includes, ordering, and paging.
/// </summary>
/// <typeparam name="TEntity">The entity type this specification applies to. Must inherit from <see cref="Entity"/>.</typeparam>
/// <example>
/// <code>
/// public sealed class ActiveUsersByRole : Specification&lt;User&gt;
/// {
///     public ActiveUsersByRole(string role)
///         : base(u => u.IsActive &amp;&amp; u.Role == role)
///     {
///         Include(u => u.Contacts);
///         OrderBy(u => u.Name);
///         ApplyNoTracking();
///     }
/// }
/// </code>
/// </example>
public abstract class Specification<TEntity> : ISpecification<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Initializes a new specification with an optional filter criteria expression.
    /// </summary>
    /// <param name="criteria">The filter expression, or <c>null</c> for no filtering.</param>
    protected Specification(Expression<Func<TEntity, bool>>? criteria)
    {
        Criteria = criteria;
    }

    /// <inheritdoc />
    public Expression<Func<TEntity, bool>>? Criteria { get; }

    /// <inheritdoc />
    public List<Expression<Func<TEntity, object>>> Includes { get; } = [];

    /// <inheritdoc />
    public List<string> IncludeStrings { get; } = [];

    /// <inheritdoc />
    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }

    /// <inheritdoc />
    public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; private set; }

    /// <inheritdoc />
    public bool AsNoTracking { get; private set; }

    /// <inheritdoc />
    public int Take { get; private set; }

    /// <inheritdoc />
    public int Skip { get; private set; }

    /// <inheritdoc />
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// Adds an expression-based eager-loading include for a navigation property.
    /// </summary>
    /// <param name="includeExpression">The navigation property to include.</param>
    protected void Include(Expression<Func<TEntity, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Adds a string-based eager-loading include for a navigation property (supports nested paths like <c>"Orders.Items"</c>).
    /// </summary>
    /// <param name="includeString">The dot-separated navigation path to include.</param>
    protected void Include(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Configures the query to execute without change tracking (<c>AsNoTracking</c>). Use for read-only queries.
    /// </summary>
    protected void ApplyNoTracking()
    {
        AsNoTracking = true;
    }

    /// <summary>
    /// Sets ascending ordering for the query results.
    /// </summary>
    /// <param name="orderByExpression">The property to order by ascending.</param>
    protected void OrderBy(Expression<Func<TEntity, object>> orderByExpression)
    {
        OrderByExpression = orderByExpression;
    }

    /// <summary>
    /// Sets descending ordering for the query results.
    /// </summary>
    /// <param name="orderByDescendingExpression">The property to order by descending.</param>
    protected void OrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
    {
        OrderByDescendingExpression = orderByDescendingExpression;
    }

    /// <summary>
    /// Enables paging for the query results.
    /// </summary>
    /// <param name="skip">The number of items to skip (typically <c>(pageNumber - 1) * pageSize</c>).</param>
    /// <param name="take">The maximum number of items to return (page size).</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}
