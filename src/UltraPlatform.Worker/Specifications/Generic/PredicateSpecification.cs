using System.Linq.Expressions;

namespace UltraPlatform.Worker.Specifications.Generic;

/// <summary>
/// Generic specification for custom predicates.
/// Allows wrapping any lambda as a reusable specification.
/// </summary>
public class PredicateSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _predicate;
    private readonly string? _description;

    public PredicateSpecification(Expression<Func<T, bool>> predicate, string? description = null)
    {
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        _description = description;
    }

    public override IQueryable<T> Apply(IQueryable<T> query)
    {
        return query.Where(_predicate);
    }

    public override string ToString()
    {
        return _description ?? $"PredicateSpecification<{typeof(T).Name}>";
    }
}
