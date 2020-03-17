using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using payment_gateway_repository.Engine.Contract;
using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;

namespace payment_gateway_repository.Repository
{
    public class UserRepository : BaseRepository, IRepository<User>
    {
        public UserRepository(INonSqlDataSource nonSqlDataSource) : base(nonSqlDataSource, "payment-gateway", "user") { }

        public User GetItem(Expression<Func<User, bool>> filter)
            => NonSqlDataSource.GetItem(NonSqlSchema, filter);

        public Task<User> GetItemAsync(Expression<Func<User, bool>> filter)
            => NonSqlDataSource.GetItemAsync(NonSqlSchema, filter);

        public IEnumerable<User> GetItems(Expression<Func<User, bool>> filter)
            => NonSqlDataSource.GetItems(NonSqlSchema, filter);

        public IQueryable<User> GetMongoQueryable()
            => NonSqlDataSource.GetMongoQueryable<User>(NonSqlSchema);

        public bool AddItem(User value)
            => NonSqlDataSource.Insert(NonSqlSchema, value);

        public Task<bool> AddItemAsync(User value)
            => NonSqlDataSource.InsertAsync(NonSqlSchema, value);

        public bool UpdateItem(User value, IDictionary<string, object> parameters)
            => NonSqlDataSource.Update<User>(NonSqlSchema, x => x.UserId == value.UserId, parameters);

        public bool UpdateItem<TField>(User value, Expression<Func<User, TField>> updateExp, TField tvalue)
            => NonSqlDataSource.Update(NonSqlSchema, x => x.UserId == value.UserId, updateExp, tvalue);

        public Task<bool> RemoveItemAsync(User value)
            => NonSqlDataSource.DeleteAsync<User>(NonSqlSchema, x => x.UserId == value.UserId);

        public bool Replace(Expression<Func<User, bool>> filter, User newValue)
            => NonSqlDataSource.Replace(NonSqlSchema, filter, newValue);

        public Task<bool> ReplaceAsync(Expression<Func<User, bool>> filter, User newValue)
            => NonSqlDataSource.ReplaceAsync(NonSqlSchema, filter, newValue);
    }
}
