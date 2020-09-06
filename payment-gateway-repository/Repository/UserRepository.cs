using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using payment_gateway_repository.Engine.Contract;
using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway_repository.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(INonSqlDataSource nonSqlDataSource) : base(nonSqlDataSource, "payment-gateway", "user") { }

        public User GetItem(Expression<Func<User, bool>> filter)
            => NonSqlDataSource.GetItem(NonSqlSchema, filter);

        public Task<User> GetItemAsync(Expression<Func<User, bool>> filter)
            => NonSqlDataSource.GetItemAsync(NonSqlSchema, filter);

        public IQueryable<User> GetMongoQueryable()
            => NonSqlDataSource.GetMongoQueryable<User>(NonSqlSchema);

        public Task<bool> AddItemAsync(User value)
            => NonSqlDataSource.InsertAsync(NonSqlSchema, value);

        public Task<bool> UpdateItemAsync<TField>(User value, Expression<Func<User, TField>> updateExp, TField tvalue)
            => NonSqlDataSource.UpdateAsync(NonSqlSchema, x => x.UserId == value.UserId, updateExp, tvalue);
    }
}
