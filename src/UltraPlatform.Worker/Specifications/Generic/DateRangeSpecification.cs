namespace UltraPlatform.Worker.Specifications.Generic;

/// <summary>
/// Generic specification for filtering entities with a Date property within a range.
/// Can be used with any type T that has a Date property (DateOnly or DateTime).
/// </summary>
public class DateRangeSpecification<T> : Specification<T> where T : class
{
    private readonly DateOnly _startDate;
    private readonly DateOnly _endDate;
    private readonly Func<T, DateOnly> _dateSelector;

    public DateRangeSpecification(DateOnly startDate, DateOnly endDate, Func<T, DateOnly> dateSelector)
    {
        _startDate = startDate;
        _endDate = endDate;
        _dateSelector = dateSelector ?? throw new ArgumentNullException(nameof(dateSelector));
    }

    public override IEnumerable<T> Apply(IEnumerable<T> query)
    {
        return query.Where(item =>
        {
            var date = _dateSelector(item);
            return date >= _startDate && date <= _endDate;
        });
    }
}
