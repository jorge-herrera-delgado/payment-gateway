using payment_gateway_repository.Engine.Contract;
using payment_gateway_repository.Engine.Model;

namespace payment_gateway_repository.Engine.Repository
{
    public abstract class BaseRepository
    {
        protected readonly INonSqlDataSource NonSqlDataSource;
        protected readonly NonSqlSchema NonSqlSchema;

        protected BaseRepository(INonSqlDataSource nonSqlDataSource, string dbName, string collName)
        {
            NonSqlSchema = new NonSqlSchema(dbName, collName);
            NonSqlDataSource = nonSqlDataSource;
        }
    }
}