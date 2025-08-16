using System.Data.SqlTypes;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;
using static rinha_backend.Requests;
using static rinha_backend.Responses;

namespace rinha_backend
{
    internal class Responses
    {
        internal record struct PaymentSummary
        {
            public bool IsDefault { get; set; }

            [JsonPropertyName("totalRequests")]
            public int TotalRequests { get; set; }

            [JsonPropertyName("totalAmount")]
            public decimal TotalAmount { get; set; }

            public PaymentSummary(bool isDefault, int totalRequests, decimal totalAmount)
            {
                IsDefault = isDefault;
                TotalRequests = totalRequests;
                TotalAmount = totalAmount;
            }

            public PaymentSummary()
            {
                IsDefault = true;
                TotalRequests = 0;
                TotalAmount = 0.0m;
            }
        }

        internal record struct PaymentItem
        {
            [JsonPropertyName("totalRequests")]
            public int TotalRequests { get; set; }

            [JsonPropertyName("totalAmount")]
            public decimal TotalAmount { get; set; }

            public PaymentItem(int totalRequests, decimal totalAmount)
            {
                TotalRequests = totalRequests;
                TotalAmount = totalAmount;
            }
        }
        internal record struct PaymentsSummaryResponse
        {
            [JsonPropertyName("default")]
            public PaymentItem Default { get; set; }

            [JsonPropertyName("fallback")]
            public PaymentItem Fallback { get; set; }

            public PaymentsSummaryResponse(PaymentItem @default, PaymentItem fallback)
            {
                Default = @default;
                Fallback = fallback;
            }
        }
    }
}
