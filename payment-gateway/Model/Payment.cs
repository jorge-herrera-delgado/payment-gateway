using Newtonsoft.Json;

namespace payment_gateway.Model
{
    public class Payment
    {
        [JsonProperty("amount")] public decimal Amount { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }
    }
}