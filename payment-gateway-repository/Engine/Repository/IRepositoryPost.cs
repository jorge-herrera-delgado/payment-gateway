using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace payment_gateway_repository.Engine.Repository
{
    public interface IRepositoryPost<T>
    {
        bool AddItem(T value);
        Task<bool> AddItemAsync(T value);
        bool UpdateItem(T value, IDictionary<string, object> parameters);
        bool UpdateItem<TField>(T model, Expression<Func<T, TField>> updateExp, TField value);
        Task<bool> RemoveItemAsync(T value);
        bool Replace(Expression<Func<T, bool>> filter, T newValue);
        Task<bool> ReplaceAsync(Expression<Func<T, bool>> filter, T newValue);
    }
}