using System;
using Newtonsoft.Json;

namespace payment_gateway.Model
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class UserPayment
    {
        [JsonProperty("user-payment-id")] public Guid UserPaymentId { get; set; } = Guid.NewGuid();
        [JsonProperty("user-id")] public Guid UserId { get; set; }
        [JsonProperty("product-id")] public string ProductId { get; set; }
        [JsonProperty("product-name")] public string ProductName { get; set; }
        [JsonProperty("details")] public string Details { get; set; }
        [JsonProperty("card-details")] public CardDetails CardDetails { get; set; }
        [JsonProperty("payment")] public Payment Payment { get; set; }
    }
}