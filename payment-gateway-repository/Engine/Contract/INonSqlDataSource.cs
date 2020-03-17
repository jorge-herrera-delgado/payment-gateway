using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using payment_gateway_repository.Engine.Model;

namespace payment_gateway_repository.Engine.Contract
{
    public interface INonSqlDataSource
    {
        IEnumerable<T> GetAll<T>(NonSqlSchema nonSqlSchema);
        Task GetAllAsync<T>(NonSqlSchema nonSqlSchema);
        T GetItem<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression);
        Task<T> GetItemAsync<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression);
        IQueryable<T> GetMongoQueryable<T>(NonSqlSchema nonSqlSchema);
        IEnumerable<T> GetItems<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression);
        Task GetItemsAsync<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression);
        IEnumerable<TNewProjection> GetItems<T, TNewProjection>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression, Expression<Func<T, TNewProjection>> projectionExpression);
        bool Insert<T>(NonSqlSchema nonSqlSchema, T obj);
        bool InsertRange<T>(NonSqlSchema nonSqlSchema, IEnumerable<T> objs);
        Task<bool> InsertAsync<T>(NonSqlSchema nonSqlSchema, T obj);
        Task<bool> InsertRangeAsync<T>(NonSqlSchema nonSqlSchema, IEnumerable<T> objs);
        bool Delete<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter);
        Task<bool> DeleteAsync<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter);
        bool DeleteRange<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter);
        Task<bool> DeleteRangeAsync<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter);
        bool Update<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter, IDictionary<string, object> updateParameters);
        bool Update<T, TField>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter, Expression<Func<T, TField>> updateExp, TField value);
        bool Replace<TDocument>(NonSqlSchema nonSqlSchema, Expression<Func<TDocument, bool>> filter, TDocument newDocument);
        Task<bool> ReplaceAsync<TDocument>(NonSqlSchema nonSqlSchema, Expression<Func<TDocument, bool>> filter, TDocument newDocument);
    }
}
