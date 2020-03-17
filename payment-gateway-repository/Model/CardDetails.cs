using MongoDB.Bson.Serialization.Attributes;

namespace payment_gateway_repository.Model
{
    public class CardDetails
    {
        [BsonElement("card-firstname")] public string CardFirstname { get; set; }
        [BsonElement("card-lastname")] public string CardLastname { get; set; }
        [BsonElement("card-number")] public string CardNumber { get; set; }
        [BsonElement("expiry-date-month")] public int ExpiryDateMonth { get; set; }
        [BsonElement("expiry-date-year")] public int ExpiryDateYear { get; set; }
        [BsonIgnore] public string Cvv { get; set; }
        [BsonElement("type")] public string Type { get; set; }
    }
}