using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using payment_gateway_repository.Model;

namespace payment_gateway_repository.Repository.Contract
{
    public interface IUserRepository
    {
        User GetItem(Expression<Func<User, bool>> filter);
        Task<User> GetItemAsync(Expression<Func<User, bool>> filter);
        IQueryable<User> GetMongoQueryable();
        Task<bool> AddItemAsync(User value);
        Task<bool> UpdateItemAsync<TField>(User value, Expression<Func<User, TField>> updateExp, TField tvalue);
    }
}
