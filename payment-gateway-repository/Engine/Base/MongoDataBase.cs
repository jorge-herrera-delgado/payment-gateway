using MongoDB.Driver;

namespace payment_gateway_repository.Engine.Base
{
    public sealed class MongoDataBase : NonSqlBase<MongoClient>
    {
        public override MongoClient OpenConnection()
            => new MongoClient(Client.ConnectionString);
    }
}