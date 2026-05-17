namespace UltraPlatform.Worker.Specifications;

/// <summary>
/// Specification pattern using IQueryable — EF Core translates to SQL.
/// Specs only define filtering criteria, not business decisions.
/// </summary>
public interface ISpecification<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}
