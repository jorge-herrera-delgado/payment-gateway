﻿using MongoDB.Bson.Serialization.Attributes;

namespace payment_gateway_repository.Model
{
    public class Payment
    {
        [BsonElement("amount")] public decimal Amount { get; set; }
        [BsonElement("currency")] public string Currency { get; set; }
    }
}