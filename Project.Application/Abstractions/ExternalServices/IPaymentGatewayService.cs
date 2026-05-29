namespace Project.Application.Abstractions.ExternalServices
{
    public interface IPaymentGatewayService
    {
        Task<GatewayOrderResult> CreateOrderAsync(decimal amount, string currency, string? receipt, CancellationToken ct = default);
        bool VerifySignature(string gatewayOrderId, string gatewayPaymentId, string signature);
    }

    public sealed record GatewayOrderResult(string GatewayOrderId, decimal Amount, string Currency, string PublicKeyId);
}
