namespace InyeccionDependencias.Models;

public interface IRequestContext
{
    Guid RequestId { get; }
    DateTime RequestTime { get; }
}