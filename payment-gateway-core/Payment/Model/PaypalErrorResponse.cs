using System.Collections.Generic;
using Newtonsoft.Json;

namespace payment_gateway_core.Payment.Model
{
    public class Detail
    {
        [JsonProperty("field")] public string Field { get; set; }
        [JsonProperty("issue")] public string Issue { get; set; }
    }

    public class PaypalErrorResponse
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("details")] public List<Detail> Details { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("information_link")] public string InformationLink { get; set; }
        [JsonProperty("debug_id")] public string DebugId { get; set; }
    }
}
