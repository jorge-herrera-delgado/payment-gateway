using System;
using Newtonsoft.Json;

namespace payment_gateway.Model
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class User
    {
        [JsonProperty("user-id")] public Guid UserId { get; set; } = Guid.NewGuid();
        [JsonProperty("firstname")] public string FirstName { get; set; }
        [JsonProperty("lastname")] public string LastName { get; set; }
        [JsonProperty("login")] public UserLogin UserLogin { get; set; }
    }
}
