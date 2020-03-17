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
    public class UserPaymentsRepository : BaseRepository, IRepository<UserPayment>
    {
        public UserPaymentsRepository(INonSqlDataSource nonSqlDataSource) : base(nonSqlDataSource, "payment-gateway", "user-payments") { }

        public UserPayment GetItem(Expression<Func<UserPayment, bool>> filter)
            => NonSqlDataSource.GetItem(NonSqlSchema, filter);

        public Task<UserPayment> GetItemAsync(Expression<Func<UserPayment, bool>> filter)
            => NonSqlDataSource.GetItemAsync(NonSqlSchema, filter);

        public IEnumerable<UserPayment> GetItems(Expression<Func<UserPayment, bool>> filter)
            => NonSqlDataSource.GetItems(NonSqlSchema, filter);

        public IQueryable<UserPayment> GetMongoQueryable()
            => NonSqlDataSource.GetMongoQueryable<UserPayment>(NonSqlSchema);

        public bool AddItem(UserPayment value)
            => NonSqlDataSource.Insert(NonSqlSchema, value);

        public Task<bool> AddItemAsync(UserPayment value)
            => NonSqlDataSource.InsertAsync(NonSqlSchema, value);

        public bool UpdateItem(UserPayment value, IDictionary<string, object> parameters)
            => NonSqlDataSource.Update<UserPayment>(NonSqlSchema, x => x.UserId == value.UserId, parameters);

        public bool UpdateItem<TField>(UserPayment value, Expression<Func<UserPayment, TField>> updateExp, TField tvalue)
            => NonSqlDataSource.Update(NonSqlSchema, x => x.UserId == value.UserId, updateExp, tvalue);

        public Task<bool> RemoveItemAsync(UserPayment value)
            => NonSqlDataSource.DeleteAsync<UserPayment>(NonSqlSchema, x => x.UserId == value.UserId);

        public bool Replace(Expression<Func<UserPayment, bool>> filter, UserPayment newValue)
            => NonSqlDataSource.Replace(NonSqlSchema, filter, newValue);

        public Task<bool> ReplaceAsync(Expression<Func<UserPayment, bool>> filter, UserPayment newValue)
            => NonSqlDataSource.ReplaceAsync(NonSqlSchema, filter, newValue);
    }
}
