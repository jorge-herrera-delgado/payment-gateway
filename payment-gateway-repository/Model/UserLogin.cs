using MongoDB.Bson.Serialization.Attributes;

namespace payment_gateway_repository.Model
{
    public class UserLogin
    {
        [BsonElement("username")] public string Username { get; set; }
        [BsonElement("password")] public string Password { get; set; }
        [BsonElement("token")] public string Token { get; set; }
    }
}