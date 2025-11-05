namespace InyeccionDependencias.Models;

public class RequestContext : IRequestContext
{
    public Guid RequestId { get; }
    public DateTime RequestTime { get; }

    public RequestContext()
    {
        RequestId = Guid.NewGuid();
        RequestTime = DateTime.UtcNow;
        Console.WriteLine($"RequestContext creado: {RequestId}");
    }
}