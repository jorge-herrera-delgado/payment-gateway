using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Authentication;
using System.Threading.Tasks;
using MongoDB.Driver;
using payment_gateway_repository.Engine.Contract;
using payment_gateway_repository.Engine.Model;

namespace payment_gateway_repository.Engine.Base
{
    public class MongoRepositoryBase : INonSqlDataSource
    {
        private readonly MongoClient _mongoClient;
        public MongoRepositoryBase(string connectionString)
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.None };
            _mongoClient = new MongoDataBase().OpenConnection(settings);
        }

        #region Public Methods

        public IEnumerable<T> GetAll<T>(NonSqlSchema nonSqlSchema)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            return collection.Find(FilterDefinition<T>.Empty).ToList();
        }

        public Task GetAllAsync<T>(NonSqlSchema nonSqlSchema)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            return collection.FindAsync(FilterDefinition<T>.Empty);
        }

        public T GetItem<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            return collection.Find(filterExpression).FirstOrDefault();
        }

        public async Task<T> GetItemAsync<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            return await collection.Find(filterExpression).FirstOrDefaultAsync();
        }

        public IQueryable<T> GetMongoQueryable<T>(NonSqlSchema nonSqlSchema)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            return collection.AsQueryable().AsQueryable();
        }

        public Task GetItemsAsync<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            return collection.FindAsync(filterExpression);
        }

        public IEnumerable<T> GetItems<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            return collection.Find(filterExpression).ToList();
        }

        public IEnumerable<TNewProjection> GetItems<T, TNewProjection>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filterExpression, Expression<Func<T, TNewProjection>> projectionExpression)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            return collection.Find(filterExpression).Project(projectionExpression).ToList();
        }

        public bool Insert<T>(NonSqlSchema nonSqlSchema, T obj)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            collection.InsertOne(obj);
            return true;
        }

        public async Task<bool> InsertAsync<T>(NonSqlSchema nonSqlSchema, T obj)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            await collection.InsertOneAsync(obj);
            return true;
        }

        public bool InsertRange<T>(NonSqlSchema nonSqlSchema, IEnumerable<T> objs)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            collection.InsertMany(objs);
            return true;
        }

        public async Task<bool> InsertRangeAsync<T>(NonSqlSchema nonSqlSchema, IEnumerable<T> objs)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            await collection.InsertManyAsync(objs);
            return true;
        }

        public bool DeleteRange<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            var status = collection.DeleteMany(filter);
            return status.IsAcknowledged && status.DeletedCount > 0;
        }

        public Task<bool> DeleteRangeAsync<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            var status = collection.DeleteManyAsync(filter).Result;
            return Task.FromResult(status.IsAcknowledged && status.DeletedCount > 0);
        }

        public bool Delete<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            var status = collection.DeleteOne(filter);
            return status.IsAcknowledged && status.DeletedCount > 0;
        }

        public Task<bool> DeleteAsync<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            var status = collection.DeleteOneAsync(filter).Result;
            return Task.FromResult(status.IsAcknowledged && status.DeletedCount > 0);
        }

        public bool Update<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter, IDictionary<string, object> updateParameters)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            var status = collection.UpdateOne(filter, GetUpdateDefinition<T>(updateParameters));
            return status.IsAcknowledged && status.ModifiedCount > 0;
        }

        public bool Update<T, TField>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter, Expression<Func<T, TField>> updateExp, TField value)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            var status = collection.UpdateOne(filter, GetUpdateDefinition(updateExp, value));
            return status.IsAcknowledged && status.ModifiedCount > 0;
        }

        public Task<bool> UpdateAsync<T>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter, IDictionary<string, object> updateParameters)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            var status = collection.UpdateOneAsync(filter, GetUpdateDefinition<T>(updateParameters)).Result;
            return Task.FromResult(status.IsAcknowledged && status.ModifiedCount > 0);
        }

        public Task<bool> UpdateAsync<T, TField>(NonSqlSchema nonSqlSchema, Expression<Func<T, bool>> filter, Expression<Func<T, TField>> updateExp, TField value)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<T>(nonSqlSchema.CollectionName);
            var status = collection.UpdateOneAsync(filter, GetUpdateDefinition(updateExp, value)).Result;
            return Task.FromResult(status.IsAcknowledged && status.ModifiedCount > 0);
        }

        public bool Replace<TDocument>(NonSqlSchema nonSqlSchema, Expression<Func<TDocument, bool>> filter, TDocument newDocument)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<TDocument>(nonSqlSchema.CollectionName);
            var status = collection.ReplaceOne(filter, newDocument);
            return status.IsAcknowledged && status.ModifiedCount > 0;
        }

        public Task<bool> ReplaceAsync<TDocument>(NonSqlSchema nonSqlSchema, Expression<Func<TDocument, bool>> filter, TDocument newDocument)
        {
            var mDb = _mongoClient.GetDatabase(nonSqlSchema.DataBaseName);
            var collection = mDb.GetCollection<TDocument>(nonSqlSchema.CollectionName);
            var status = collection.ReplaceOneAsync(filter, newDocument).Result;
            return Task.FromResult(status.IsAcknowledged && status.ModifiedCount > 0);
        }

        #endregion

        #region Private Methods

        private static UpdateDefinition<T> GetUpdateDefinition<T>(IDictionary<string, object> updateParameters)
        {
            var updateDefinitionBuilder = Builders<T>.Update;
            if (updateParameters == null || updateParameters.Count <= 0)
                throw new ArgumentNullException(nameof(updateParameters));

            var fieldsToBeUpdated = updateParameters.Select(entry => updateDefinitionBuilder.Set(entry.Key, entry.Value)).ToList();
            return updateDefinitionBuilder.Combine(fieldsToBeUpdated);
        }

        private static UpdateDefinition<T> GetUpdateDefinition<T, TField>(Expression<Func<T, TField>> filter, TField value)
        {
            var updateDefinitionBuilder = Builders<T>.Update;
            return filter != null
                ? updateDefinitionBuilder.Set(filter, value)
                : throw new ArgumentNullException(nameof(value));
        }

        #endregion
    }


}

