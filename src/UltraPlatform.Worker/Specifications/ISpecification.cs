using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Specifications;

/// <summary>
/// Specification pattern for encapsulating query logic.
/// Non-generic version for backward compatibility.
/// </summary>
public interface ISpecification
{
    IEnumerable<DataRecord> Apply(IEnumerable<DataRecord> query);
}

/// <summary>
/// Generic Specification pattern for encapsulating query logic on any type.
/// Allows combining multiple filter criteria in a safe, composable way.
/// </summary>
public interface ISpecification<T> where T : class
{
    /// <summary>
    /// Applies the specification filter to a queryable source.
    /// </summary>
    IEnumerable<T> Apply(IEnumerable<T> query);
}
