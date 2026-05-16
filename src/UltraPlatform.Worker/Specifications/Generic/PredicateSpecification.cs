namespace UltraPlatform.Worker.Specifications.Generic;

/// <summary>
/// Generic specification for custom predicates.
/// Allows wrapping any lambda as a reusable specification.
/// </summary>
public class PredicateSpecification<T> : Specification<T> where T : class
{
    private readonly Func<T, bool> _predicate;
    private readonly string? _description;

    public PredicateSpecification(Func<T, bool> predicate, string? description = null)
    {
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        _description = description;
    }

    public override IEnumerable<T> Apply(IEnumerable<T> query)
    {
        return query.Where(_predicate);
    }

    public override string ToString()
    {
        return _description ?? $"PredicateSpecification<{typeof(T).Name}>";
    }
}
