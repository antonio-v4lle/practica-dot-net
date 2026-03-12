using Payments.Core.DTOs;
using Payments.Core.Interfaces;
using Payments.Core.Services;
using Payments.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ITransactionRepository, InMemoryTransactionRepository>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/payments/charges", async (CreateTransactionRequest request, IPaymentService service) =>
{
    try
    {
        var result = await service.CreateChargeAsync(request);
        return Results.Created($"/payments/transactions/{result.Id}", result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateCharge")
.WithTags("Payments");

app.MapPost("/payments/payments", async (CreateTransactionRequest request, IPaymentService service) =>
{
    try
    {
        var result = await service.CreatePaymentAsync(request);
        return Results.Created($"/payments/transactions/{result.Id}", result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreatePayment")
.WithTags("Payments");

app.MapGet("/payments/balance", async (IPaymentService service) =>
{
    var balance = await service.GetBalanceAsync();
    return Results.Ok(balance);
})
.WithName("GetBalance")
.WithTags("Payments");

app.MapGet("/payments/transactions", async (IPaymentService service) =>
{
    var transactions = await service.GetTransactionsAsync();
    return Results.Ok(transactions);
})
.WithName("GetTransactions")
.WithTags("Payments");

app.Run();
