using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace payment_gateway.Model
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class UserLogin
    {
        [JsonProperty("username")] [Required] public string Username { get; set; }
        [JsonProperty("password")] [Required] public string Password { get; set; }
        [JsonProperty("token")] public string Token { get; set; }
    }
}