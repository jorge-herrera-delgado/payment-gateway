using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using payment_gateway_repository.Engine.Contract;
using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway_repository.Repository
{
    public class PaymentRepository : BaseRepository, IPaymentRepository
    {
        public PaymentRepository(INonSqlDataSource nonSqlDataSource) : base(nonSqlDataSource, "payment-gateway", "user-payments") { }

        public Payment GetItem(Expression<Func<Payment, bool>> filter)
            => NonSqlDataSource.GetItem(NonSqlSchema, filter);

        public Task<Payment> GetItemAsync(Expression<Func<Payment, bool>> filter)
            => NonSqlDataSource.GetItemAsync(NonSqlSchema, filter);

        public IQueryable<Payment> GetMongoQueryable()
            => NonSqlDataSource.GetMongoQueryable<Payment>(NonSqlSchema);

        public Task<bool> AddItemAsync(Payment value)
            => NonSqlDataSource.InsertAsync(NonSqlSchema, value);
    }
}
