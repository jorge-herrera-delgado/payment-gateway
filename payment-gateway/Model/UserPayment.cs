using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace payment_gateway.Model
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class UserPayment
    {
        [JsonProperty("user-payment-id")] public Guid UserPaymentId { get; set; } = Guid.NewGuid();
        [JsonProperty("user-id")] [Required] public Guid UserId { get; set; }
        [JsonProperty("product-id")] [Required] public string ProductId { get; set; }
        [JsonProperty("product-name")] [Required] public string ProductName { get; set; }
        [JsonProperty("details")] [Required] public string Details { get; set; }
        [JsonProperty("card-details")] [Required] public CardDetails CardDetails { get; set; }
        [JsonProperty("payment")] [Required] public Payment Payment { get; set; }
        [JsonProperty("created")] public DateTime Created { get; set; } = DateTime.Now;
    }
}