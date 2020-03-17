using System.ComponentModel;
using Newtonsoft.Json;

namespace payment_gateway_core.Validation.Engine
{
    public class Result
    {
        [JsonIgnore] public StatusCode StatusCode { get; set; } = StatusCode.Success;
        [JsonProperty("status")] public string Status => StatusCode.ToString();

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "error-code")]
        public ErrorCode ErrorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "status-detail")]
        [DefaultValue("")]
        public string StatusDetail { get; set; }
    }
}