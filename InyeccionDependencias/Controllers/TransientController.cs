using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InyeccionDependencias.Models;

namespace InyeccionDependencias.Controllers;

public class TransientController : Controller
{
    [HttpGet("test-transient")]
    public IActionResult TestTransient(
        [FromServices] IGuidGenerator gen1,
        [FromServices] IGuidGenerator gen2
    )
    {
        return Ok(new
        {
            Gen1 = gen1.GetGuid(),
            Gen2 = gen2.GetGuid(),
            Same = gen1.GetGuid() == gen2.GetGuid(),
        });
    }
}
