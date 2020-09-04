using MongoDB.Driver;

namespace payment_gateway_repository.Engine.Base
{
    public sealed class MongoDataBase : NonSqlBase<MongoClient, MongoClientSettings>
    {
        public override MongoClient OpenConnection(MongoClientSettings settings)
            => new MongoClient(settings);
    }
}