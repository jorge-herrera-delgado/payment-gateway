using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace payment_gateway.Model
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class User
    {
        [JsonProperty("user-id")] public Guid UserId { get; set; } = Guid.NewGuid();
        [JsonProperty("firstname")] [Required] public string FirstName { get; set; }
        [JsonProperty("lastname")] [Required] public string LastName { get; set; }
        [JsonProperty("login")] [Required] public UserLogin UserLogin { get; set; }
    }
}
