using System.Linq.Expressions;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Models;

namespace UltraPlatform.Worker.Specifications.Data;

public class DataActiveFilter : ISpecification<DataRecord, DateOnly>
{
    Expression<Func<DataRecord, DateOnly, IQueryable>> ISpecification<DataRecord, DateOnly>.Apply()
    {
        throw new NotImplementedException();
    }
}