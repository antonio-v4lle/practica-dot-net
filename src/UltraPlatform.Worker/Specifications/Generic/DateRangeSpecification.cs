namespace UltraPlatform.Worker.Specifications.Generic;

/// <summary>
/// Generic specification for filtering entities with a Date property within a range.
/// Can be used with any type T that has a Date property (DateOnly or DateTime).
/// </summary>
public class DateRangeSpecification<T> : Specification<T>
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

    public override IQueryable<T> Apply(IQueryable<T> query) {
      return query.Where(item => _dateSelector(item) >= _startDate && _dateSelector(item) <= _endDate);  
    } 
}
