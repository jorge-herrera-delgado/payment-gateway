using Newtonsoft.Json;

namespace payment_gateway.Model
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CardDetails
    {
        [JsonProperty("card-firstname")] public string CardFirstname { get; set; }
        [JsonProperty("card-lastname")] public string CardLastname { get; set; }
        [JsonProperty("card-number")] public string CardNumber { get; set; }
        [JsonProperty("expiry-date-month")] public int ExpiryDateMonth { get; set; }
        [JsonProperty("expiry-date-year")] public int ExpiryDateYear { get; set; }
        [JsonProperty("cvv")] public string Cvv { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
    }
}