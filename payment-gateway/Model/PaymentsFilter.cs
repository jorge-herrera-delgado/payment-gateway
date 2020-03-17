using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace payment_gateway.Model
{
    public class PaymentsFilter
    {
        [JsonProperty("user-id")] [Required] public string UserId { get; set; }
        [JsonProperty("start-date")] public DateTime StartDate { get; set; }
        [JsonProperty("end-date")] public DateTime EndDate { get; set; }
        [JsonProperty("product-id")] public string ProductId { get; set; }
        [JsonProperty("product-name")] public string ProductName { get; set; }
        [JsonProperty("card-number")] public string CardNumber { get; set; }

    }
}
