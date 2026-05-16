using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Specifications;

/// <summary>
/// Base class for specifications. Can be composed with AND/OR logic.
/// </summary>
public abstract class Specification : ISpecification
{
    public abstract IEnumerable<DataRecord> Apply(IEnumerable<DataRecord> query);

    public Specification And(ISpecification other)
    {
        return new AndSpecification(this, other);
    }

    public Specification Or(ISpecification other)
    {
        return new OrSpecification(this, other);
    }

    public Specification Not()
    {
        return new NotSpecification(this);
    }
}

/// <summary>
/// Combines two specifications with AND logic.
/// </summary>
internal class AndSpecification : Specification
{
    private readonly ISpecification _left;
    private readonly ISpecification _right;

    public AndSpecification(ISpecification left, ISpecification right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override IEnumerable<DataRecord> Apply(IEnumerable<DataRecord> query)
    {
        var leftResult = _left.Apply(query);
        return _right.Apply(leftResult);
    }
}

/// <summary>
/// Combines two specifications with OR logic.
/// </summary>
internal class OrSpecification : Specification
{
    private readonly ISpecification _left;
    private readonly ISpecification _right;

    public OrSpecification(ISpecification left, ISpecification right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override IEnumerable<DataRecord> Apply(IEnumerable<DataRecord> query)
    {
        var leftResult = _left.Apply(query).ToList();
        var rightResult = _right.Apply(query).ToList();
        return leftResult.Union(rightResult);
    }
}

/// <summary>
/// Negates a specification (NOT logic).
/// </summary>
internal class NotSpecification : Specification
{
    private readonly ISpecification _specification;

    public NotSpecification(ISpecification specification)
    {
        _specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    public override IEnumerable<DataRecord> Apply(IEnumerable<DataRecord> query)
    {
        var specified = _specification.Apply(query).ToHashSet();
        return query.Except(specified);
    }
}
