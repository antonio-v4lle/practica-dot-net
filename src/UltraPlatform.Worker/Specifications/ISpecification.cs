using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Specifications;

/// <summary>
/// Specification pattern for encapsulating query logic.
/// Allows combining multiple filter criteria in a safe, composable way.
/// </summary>
public interface ISpecification
{
    /// <summary>
    /// Applies the specification filter to a queryable source.
    /// </summary>
    IEnumerable<DataRecord> Apply(IEnumerable<DataRecord> query);
}
