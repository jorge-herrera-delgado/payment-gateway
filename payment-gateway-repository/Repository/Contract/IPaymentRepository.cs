using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;

namespace payment_gateway_repository.Repository.Contract
{
    public interface IPaymentRepository
    {
        Payment GetItem(Expression<Func<Payment, bool>> filter);
        Task<Payment> GetItemAsync(Expression<Func<Payment, bool>> filter);
        IQueryable<Payment> GetMongoQueryable();
        Task<bool> AddItemAsync(Payment value);
    }
}
