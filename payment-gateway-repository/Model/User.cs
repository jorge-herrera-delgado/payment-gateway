using System;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace payment_gateway_repository.Model
{
    public class User
    {
        [BsonId, BsonRepresentation(BsonType.String)] public Guid UserId { get; set; } = Guid.NewGuid();
        [BsonElement("firstname")] public string FirstName { get; set; }
        [BsonElement("lastname")] public string LastName { get; set; }
        [BsonElement("login")] public UserLogin UserLogin { get; set; }
    }
}
