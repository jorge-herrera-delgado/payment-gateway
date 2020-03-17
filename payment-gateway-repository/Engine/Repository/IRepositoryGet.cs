using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace payment_gateway_repository.Engine.Repository
{
    public interface IRepositoryGet<T>
    {
        T GetItem(Expression<Func<T, bool>> filter);
        Task<T> GetItemAsync(Expression<Func<T, bool>> filter);
        IEnumerable<T> GetItems(Expression<Func<T, bool>> filter);
        IQueryable<T> GetMongoQueryable();
    }
}