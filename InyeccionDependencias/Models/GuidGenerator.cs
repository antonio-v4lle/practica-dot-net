namespace InyeccionDependencias.Models;

public class GuidGenerator : IGuidGenerator
{
    private readonly Guid _instanceId;

    public GuidGenerator()
    {
        _instanceId = Guid.NewGuid();
        Console.WriteLine($"GuidGenerator creado: {_instanceId}");
    }

    public Guid GetGuid() => _instanceId;
}