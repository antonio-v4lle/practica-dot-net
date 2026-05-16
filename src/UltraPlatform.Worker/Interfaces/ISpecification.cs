using System.Linq.Expressions;

namespace UltraPlatform.Worker.Interfaces;

interface ISpecification<T, K>
{
    Expression<Func<T,K, IQueryable>> Apply();
}