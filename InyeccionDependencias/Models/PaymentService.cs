namespace InyeccionDependencias.Models;

public class PaymentService : ICheckeableRequestId
{
    private readonly IRequestContext _context;

    public PaymentService(IRequestContext context)
    {
        _context = context;
    }

    public void ProcessPayment()
    {
        Console.WriteLine($"PaymentService usando request: {_context.RequestId}");
        // Mismo RequestId que OrderService!
    }

    public Guid GetRequestId()
    {
        return _context.RequestId;
    }
}