namespace UltraPlatform.Worker.Specifications;

/// <summary>
/// Generic base class for specifications. Can be composed with AND/OR logic.
/// </summary>
public abstract class Specification<T> : ISpecification<T> where T : class
{
    public abstract IEnumerable<T> Apply(IEnumerable<T> query);

    public Specification<T> And(ISpecification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }

    public Specification<T> Or(ISpecification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

/// <summary>
/// Combines two specifications with AND logic.
/// </summary>
internal class AndSpecification<T> : Specification<T> where T : class
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override IEnumerable<T> Apply(IEnumerable<T> query)
    {
        var leftResult = _left.Apply(query);
        return _right.Apply(leftResult);
    }
}

/// <summary>
/// Combines two specifications with OR logic.
/// </summary>
internal class OrSpecification<T> : Specification<T> where T : class
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override IEnumerable<T> Apply(IEnumerable<T> query)
    {
        var leftResult = _left.Apply(query).ToList();
        var rightResult = _right.Apply(query).ToList();
        return leftResult.Union(rightResult);
    }
}

/// <summary>
/// Negates a specification (NOT logic).
/// </summary>
internal class NotSpecification<T> : Specification<T> where T : class
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    public override IEnumerable<T> Apply(IEnumerable<T> query)
    {
        var specified = _specification.Apply(query).ToHashSet();
        return query.Except(specified);
    }
}
