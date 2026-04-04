using FamilyFinance.Application;
using FamilyFinance.Api.Middleware;
using FamilyFinance.Application.Credits.Commands;
using FamilyFinance.Application.Expenses.Commands;
using FamilyFinance.Application.FamilyGroups.Commands;
using FamilyFinance.Domain.Aggregates.Expenses;
using FamilyFinance.Infrastructure;
using FamilyFinance.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FamilyFinance API",
        Version = "v1",
        Description = "API para planificación de gastos e ingresos familiares con optimización de créditos."
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FamilyFinance API v1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

await SeedDatabaseAsync(app);

app.Run();

static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<FamilyFinanceDbContext>();
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    await context.Database.MigrateAsync();

    if (await context.FamilyGroups.AnyAsync()) return;

    // --- Seed data from "Control de gastos.xlsx" ---
    var familyId = await mediator.Send(new CreateFamilyGroupCommand("Familia Principal", "MXN"));

    await mediator.Send(new AddMemberCommand(familyId, "Antonio", 42_000m));
    await mediator.Send(new AddMemberCommand(familyId, "Adriana", 17_600m));

    var group = await context.FamilyGroups.Include(g => g.Members).FirstAsync(g => g.Id == familyId);
    var antonio = group.Members.First(m => m.Name == "Antonio");

    // Gastos compartidos
    await mediator.Send(new AddExpenseCommand(familyId, "Hipoteca", ExpenseCategory.Patrimonio, 7_100m, true, 19));
    await mediator.Send(new AddExpenseCommand(familyId, "Automóvil", ExpenseCategory.Patrimonio, 11_800m, true, 4));
    await mediator.Send(new AddExpenseCommand(familyId, "Despensa I", ExpenseCategory.Alimentacion, 2_500m, true, 1));
    await mediator.Send(new AddExpenseCommand(familyId, "Despensa II", ExpenseCategory.Alimentacion, 2_500m, true, 15));
    await mediator.Send(new AddExpenseCommand(familyId, "Leonel", ExpenseCategory.Educacion, 0m, false, null, "MXN", "suspendido"));
    await mediator.Send(new AddExpenseCommand(familyId, "Boda", ExpenseCategory.Patrimonio, 0m, false, null, "MXN", "suspendido"));
    await mediator.Send(new AddExpenseCommand(familyId, "Piso", ExpenseCategory.RentasRelocaciones, 3_800m, true, 15));
    await mediator.Send(new AddExpenseCommand(familyId, "Agua", ExpenseCategory.Servicios, 250m, true, 1));
    await mediator.Send(new AddExpenseCommand(familyId, "Luz", ExpenseCategory.Servicios, 1_100m, true, 1));
    await mediator.Send(new AddExpenseCommand(familyId, "Internet", ExpenseCategory.Servicios, 1_500m, true, 1));
    await mediator.Send(new AddExpenseCommand(familyId, "Combustible", ExpenseCategory.Servicios, 4_400m, true, null, "MXN", "semanal"));

    // Suspender gastos inactivos
    var leonel = await context.SharedExpenses.FirstAsync(e => e.Description == "Leonel");
    var boda = await context.SharedExpenses.FirstAsync(e => e.Description == "Boda");
    await mediator.Send(new SuspendExpenseCommand(leonel.Id));
    await mediator.Send(new SuspendExpenseCommand(boda.Id));

    // Líneas de crédito (datos del Excel)
    await mediator.Send(new AddCreditLineCommand(
        familyId, antonio.Id, "Antonio TDC BBVA",
        137_400m, 105_357.96m, 36m, 9, 9_000m));

    await mediator.Send(new AddCreditLineCommand(
        familyId, antonio.Id, "Antonio TDC UN",
        5_000m, 3_881.31m, 42m, 20, 4_500m));

    await mediator.Send(new AddCreditLineCommand(
        familyId, antonio.Id, "Hipoteca Infonavit",
        716_000m, 606_863.40m, 10.32m, 19, 7_100m));
}
