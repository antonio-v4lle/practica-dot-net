using InyeccionDependencias.Models;
using Microsoft.AspNetCore.Mvc;
namespace InyeccionDependencias.Controllers;

public class ScopedController : Controller
{

    private readonly IRequestContext _context;
    private readonly OrderService _orderService;
    private readonly PaymentService _paymentService;

    public ScopedController(IRequestContext context, OrderService orderService, PaymentService paymentService)
    {
        _context = context;
        _orderService = orderService;
        _paymentService = paymentService;
    }

    [HttpGet("test-scoped")]
    public IActionResult TestScoped()
    {
        // Todos comparten la MISMA instancia de RequestContext
        Console.WriteLine($"Controller request: {_context.RequestId}");
        _orderService.ProcessOrder();
        _paymentService.ProcessPayment();

        // Los 3 imprimen el MISMO RequestId
        return Ok(new {
            ControllerRequestId = _context.RequestId,
            OrderServiceRequestId = _orderService.GetRequestId(),
            PaymentServiceRequestId = _paymentService.GetRequestId(),
        });
    }
}