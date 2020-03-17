using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace payment_gateway.Model
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CardDetails
    {
        [JsonProperty("card-firstname")] [Required] public string CardFirstname { get; set; }
        [JsonProperty("card-lastname")] [Required] public string CardLastname { get; set; }
        [JsonProperty("card-number")] [Required] public string CardNumber { get; set; }
        [JsonProperty("expiry-date-month")] [Required] public int ExpiryDateMonth { get; set; }
        [JsonProperty("expiry-date-year")] [Required] public int ExpiryDateYear { get; set; }
        [JsonProperty("cvv")] [Required] public string Cvv { get; set; }
        [JsonProperty("type")] [Required] public string Type { get; set; }
    }
}