using Payments.Core.DTOs;
using Payments.Core.Services;
using Payments.Infrastructure.Repositories;

namespace Payments.Test;

public class PaymentServiceTests
{
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        var repository = new InMemoryTransactionRepository();
        _service = new PaymentService(repository);
    }

    [Fact]
    public async Task CreateCharge_ValidRequest_ReturnsChargeTransaction()
    {
        var request = new CreateTransactionRequest(100m, "Test charge");

        var result = await _service.CreateChargeAsync(request);

        Assert.Equal("Charge", result.Type);
        Assert.Equal(100m, result.Amount);
        Assert.Equal("Test charge", result.Description);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task CreatePayment_ValidRequest_ReturnsPaymentTransaction()
    {
        var request = new CreateTransactionRequest(50m, "Test payment");

        var result = await _service.CreatePaymentAsync(request);

        Assert.Equal("Payment", result.Type);
        Assert.Equal(50m, result.Amount);
        Assert.Equal("Test payment", result.Description);
    }

    [Fact]
    public async Task CreateCharge_ZeroAmount_ThrowsArgumentException()
    {
        var request = new CreateTransactionRequest(0m, "Invalid");

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateChargeAsync(request));
    }

    [Fact]
    public async Task CreateCharge_NegativeAmount_ThrowsArgumentException()
    {
        var request = new CreateTransactionRequest(-10m, "Invalid");

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateChargeAsync(request));
    }

    [Fact]
    public async Task CreateCharge_EmptyDescription_ThrowsArgumentException()
    {
        var request = new CreateTransactionRequest(100m, "");

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateChargeAsync(request));
    }

    [Fact]
    public async Task GetBalance_AfterChargesAndPayments_ReturnsCorrectBalance()
    {
        await _service.CreateChargeAsync(new CreateTransactionRequest(100m, "Charge 1"));
        await _service.CreateChargeAsync(new CreateTransactionRequest(50m, "Charge 2"));
        await _service.CreatePaymentAsync(new CreateTransactionRequest(75m, "Payment 1"));

        var balance = await _service.GetBalanceAsync();

        Assert.Equal(150m, balance.TotalCharges);
        Assert.Equal(75m, balance.TotalPayments);
        Assert.Equal(75m, balance.Balance);
    }

    [Fact]
    public async Task GetTransactions_ReturnsAllTransactions()
    {
        await _service.CreateChargeAsync(new CreateTransactionRequest(100m, "Charge 1"));
        await _service.CreatePaymentAsync(new CreateTransactionRequest(50m, "Payment 1"));
        await _service.CreateChargeAsync(new CreateTransactionRequest(25m, "Charge 2"));

        var transactions = await _service.GetTransactionsAsync();

        Assert.Equal(3, transactions.Count);
    }

    [Fact]
    public async Task GetBalance_NoTransactions_ReturnsZeroes()
    {
        var balance = await _service.GetBalanceAsync();

        Assert.Equal(0m, balance.TotalCharges);
        Assert.Equal(0m, balance.TotalPayments);
        Assert.Equal(0m, balance.Balance);
    }
}
