namespace InyeccionDependencias.Models;

public class OrderService
{
    private readonly IRequestContext _context;

    public OrderService(IRequestContext context)
    {
        _context = context;
    }

    public void ProcessOrder()
    {
        Console.WriteLine($"OrderService usando request: {_context.RequestId}");
    }
}