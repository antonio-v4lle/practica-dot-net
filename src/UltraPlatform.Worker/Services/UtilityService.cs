using UltraPlatform.Worker.Database;
using UltraPlatform.Worker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace UltraPlatform.Worker.Services;

public class UtilityService : IUtilityService
{
    public DateOnly GetToday() => DateOnly.FromDateTime(DateTime.Now);
}