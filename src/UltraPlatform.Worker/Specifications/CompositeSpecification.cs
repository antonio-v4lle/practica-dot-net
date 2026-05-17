namespace UltraPlatform.Worker.Specifications;

/// <summary>
/// Abstract base — provides And/Or/Not composition for all specs.
/// </summary>
public abstract class Specification<T> : ISpecification<T>
{
    public abstract IQueryable<T> Apply(IQueryable<T> query);

    public Specification<T> And(ISpecification<T> other) => new AndSpecification<T>(this, other);
    public Specification<T> Or(ISpecification<T> other)  => new OrSpecification<T>(this, other);
    public Specification<T> Not()                        => new NotSpecification<T>(this);
}

internal class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left  = left  ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override IQueryable<T> Apply(IQueryable<T> query)
        => _right.Apply(_left.Apply(query));
}

internal class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left  = left  ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override IQueryable<T> Apply(IQueryable<T> query)
        => _left.Apply(query).Union(_right.Apply(query));
}

internal class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _inner;

    public NotSpecification(ISpecification<T> inner)
        => _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public override IQueryable<T> Apply(IQueryable<T> query)
    {
        var excluded = _inner.Apply(query);
        return query.Except(excluded);
    }
}
