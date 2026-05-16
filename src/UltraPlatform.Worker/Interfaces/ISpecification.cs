using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Interfaces;

/// <summary>
/// Specification pattern for encapsulating query logic.
/// Allows combining multiple filter criteria in a safe, composable way.
/// </summary>
public interface ISpecification<T>
{
    /// <summary>
    /// Applies the specification filter to a queryable source.
    /// </summary>
    IEnumerable<T> Apply(IEnumerable<T> query);
}
