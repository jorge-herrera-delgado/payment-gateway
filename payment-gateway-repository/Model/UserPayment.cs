using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace payment_gateway_repository.Model
{
    public class UserPayment
    {
        [BsonId, BsonRepresentation(BsonType.String)] public Guid UserPaymentId { get; set; } = Guid.NewGuid();
        [BsonElement("user-id"), BsonRepresentation(BsonType.String)] public Guid UserId { get; set; }
        [BsonElement("product-id")] public string ProductId { get; set; }
        [BsonElement("product-name")] public string ProductName { get; set; }
        [BsonElement("details")] public string Details { get; set; }
        [BsonElement("card-details")] public CardDetails CardDetails { get; set; }
        [BsonElement("payment")] public Payment Payment { get; set; }
    }
}