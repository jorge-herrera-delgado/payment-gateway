using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace payment_gateway.Model
{
    public class Price
    {
        [JsonProperty("amount")] [Required] public decimal Amount { get; set; }
        [JsonProperty("currency")] [Required] public string Currency { get; set; }
    }
}