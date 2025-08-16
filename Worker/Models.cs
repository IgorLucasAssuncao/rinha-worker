using System.Data.SqlTypes;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;
using static rinha_worker.Requests;
using static rinha_worker.Responses;

namespace rinha_worker
{
    internal class Models
    {
        internal record struct Payments
        {
            public Payments()
            {
            }

            public Guid CorrelationId { get; set; }
            public decimal Amount { get; set; }
            public bool IsDefault { get; set; } = true;
            public DateTimeOffset RequestedAt { get; set; }
        }
    }

    internal class Requests
    {
        internal record PaymentsRequest
        {
            [JsonPropertyName("correlationId")]
            public Guid CorrelationId { get; set; }

            [JsonPropertyName("amount")]
            public decimal Amount { get; set; }

            [JsonPropertyName("requestedAt")]
            public DateTimeOffset RequestedAt { get; set; }

            public PaymentsRequest(Guid correlationId, decimal amount, DateTimeOffset requestedAt)
            {
                CorrelationId = correlationId;
                Amount = amount;
                RequestedAt = requestedAt;
            }

            [JsonConstructor]
            public PaymentsRequest(Guid correlationId, decimal amount)
            {
                CorrelationId = correlationId;
                Amount = amount;
                RequestedAt = DateTimeOffset.UtcNow;
            }
        }
    }

    internal class Responses
    {
        internal record struct PaymentServiceHealth
        {
            [JsonPropertyName("failing")]
            public bool IsFailing { get; set; }

            [JsonPropertyName("minResponseTime")]
            public decimal MinResponseTime { get; set; }

            public PaymentServiceHealth(bool isFailing, decimal minResponseTime)
            {
                IsFailing = isFailing;
                MinResponseTime = minResponseTime;
            }
        }
    }
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(PaymentsRequest))]
    [JsonSerializable(typeof(PaymentServiceHealth))]
    [JsonSerializable(typeof(string))]
    internal partial class JsonContext : JsonSerializerContext
    {

    }
}
