namespace UltraPlatform.Worker.Specifications.Generic;

/// <summary>
/// Generic specification for filtering by property equality.
/// Highly reusable for any type and property.
/// </summary>
public class PropertyEqualsSpecification<T, TProp> : Specification<T> where T : class
{
    private readonly Func<T, TProp> _propertySelector;
    private readonly TProp _value;
    private readonly bool _exclude;

    public PropertyEqualsSpecification(Func<T, TProp> propertySelector, TProp value, bool exclude = false)
    {
        _propertySelector = propertySelector ?? throw new ArgumentNullException(nameof(propertySelector));
        _value = value;
        _exclude = exclude;
    }

    public override IQueryable<T> Apply(IQueryable<T> query)
    {
        if (_exclude)
            return query.Where(item => !Equals(_propertySelector(item), _value));
        
        return query.Where(item => Equals(_propertySelector(item), _value));
    }
}
