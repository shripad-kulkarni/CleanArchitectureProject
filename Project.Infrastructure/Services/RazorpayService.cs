using Microsoft.Extensions.Options;
using Project.Application.Abstractions.ExternalServices;
using Project.Infrastructure.Options;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace Project.Infrastructure.Services
{
    public sealed class RazorpayService : IPaymentGatewayService
    {
        private readonly RazorpayOptions _options;

        public RazorpayService(IOptions<RazorpayOptions> options)
        {
            _options = options.Value;
        }

        public Task<GatewayOrderResult> CreateOrderAsync(
            decimal amount, string currency, string? receipt, CancellationToken ct = default)
        {
            return Task.Run(() =>
            {
                var client = new RazorpayClient(_options.KeyId, _options.KeySecret);
                var orderOptions = new Dictionary<string, object>
                {
                    ["amount"]   = (long)(amount * 100),
                    ["currency"] = currency,
                    ["receipt"]  = receipt ?? Guid.NewGuid().ToString("N")[..16]
                };

                var order = client.Order.Create(orderOptions);
                var orderId = order["id"].ToString()!;

                return new GatewayOrderResult(orderId, amount, currency, _options.KeyId);
            }, ct);
        }

        public bool VerifySignature(string gatewayOrderId, string gatewayPaymentId, string signature)
        {
            var payload = $"{gatewayOrderId}|{gatewayPaymentId}";
            var secretBytes = Encoding.UTF8.GetBytes(_options.KeySecret);
            using var hmac = new HMACSHA256(secretBytes);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generated = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return generated == signature;
        }
    }
}
