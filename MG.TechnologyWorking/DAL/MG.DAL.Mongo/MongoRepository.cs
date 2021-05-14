using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace MG.DAL.Mongo
{
    using MG.DAL.Mongo.Interface;
    using MG.Shared.Configurations;
    using MG.Models.MongoModels.Interface;
    using MG.Models.MongoModels.Attributes;
    using Microsoft.Extensions.Logging;

    public class MongoRepository<T> : IMongoRepository<T> where T : IDocumentModel
    {
        private readonly ILogger<MongoRepository<T>> logger;
        private readonly IMongoCollection<T> collection;
        private readonly MongoDBSettings MongoDBSettings;
        private readonly IMongoDatabase MongoDatabase;

        public MongoRepository(IConfiguration configuration, ILogger<MongoRepository<T>> logger)
        {
            this.logger = logger;

            var MongoDBSettings = configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();
            this.MongoDBSettings = MongoDBSettings;

            MongoClientSettings clientSettings = Initialize(MongoDBSettings);
            MongoDatabase = new MongoClient(clientSettings).GetDatabase(MongoDBSettings.MongoDBName);

            collection = MongoDatabase.GetCollection<T>(GetCollectionName(typeof(T)));
        }

        private MongoClientSettings Initialize(MongoDBSettings mongoDbSettings)
        {
            MongoInternalIdentity internalIdentity = new MongoInternalIdentity("admin", mongoDbSettings.MongoDBUserName);
            PasswordEvidence passwordEvidence = new PasswordEvidence(mongoDbSettings.MongoDBPassword);
            MongoCredential mongoCredential = new MongoCredential(mongoDbSettings.MongoDBAuthMechanism, internalIdentity, passwordEvidence);

            MongoClientSettings clientSettings = new MongoClientSettings
            {
                Credential = mongoCredential
            };

            MongoServerAddress mongoServerAddress = new MongoServerAddress(mongoDbSettings.MongoDBHost, mongoDbSettings.MongoDBPort);
            clientSettings.Server = mongoServerAddress;

            return clientSettings;
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }

        public virtual IEnumerable<T> FilterBy(Expression<Func<T, bool>> filterExpression)
        {
            return collection.Find(filterExpression).ToEnumerable();
        }

        public virtual IEnumerable<TProjected> FilterBy<TProjected>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, TProjected>> projectionExpression)
        {
            return collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        public virtual T Find(Expression<Func<T, bool>> filterExpression)
        {
            return collection.Find(filterExpression).FirstOrDefault();
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> filterExpression)
        {
            try
            {
                var model = await collection.FindAsync(filterExpression);
                return await model.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual T FindById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
            return collection.Find(filter).SingleOrDefault();
        }

        public virtual async Task<T> FindByIdAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
            var model = await collection.FindAsync(filter);
            return await model.SingleOrDefaultAsync();
        }

        public virtual void Insert(T document)
        {
            collection.InsertOne(document);
        }

        public async virtual Task InsertChunkAsync(T document)
        {
            try
            {
                var bucket = new GridFSBucket(MongoDatabase);
                var res = await bucket.UploadFromBytesAsync(document.Id.ToString(), document.Data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual async Task InsertAsync(T document)
        {
            await collection.InsertOneAsync(document);
        }

        public virtual void InsertMany(ICollection<T> documents)
        {
            collection.InsertMany(documents);
        }

        public virtual async Task InsertManyAsync(ICollection<T> documents)
        {
            await collection.InsertManyAsync(documents);
        }

        public virtual void Update(T document)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, document.Id);
            collection.FindOneAndReplace(filter, document);
        }

        public virtual async Task UpdateAsync(T document)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, document.Id);
            await collection.FindOneAndReplaceAsync(filter, document);
        }

        public virtual void Delete(Expression<Func<T, bool>> filterExpression)
        {
            collection.DeleteOne(filterExpression);
        }

        public virtual async Task DeleteAsync(Expression<Func<T, bool>> filterExpression)
        {
            await collection.DeleteOneAsync(filterExpression);
        }

        public virtual void DeleteById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
            collection.FindOneAndDelete(filter);
        }

        public virtual async Task DeleteByIdAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
            await collection.FindOneAndDeleteAsync(filter);
        }

        /// <summary>
        /// Hard Delete
        /// </summary>
        /// <param name="filterExpression"></param>
        public virtual void DeleteMany(Expression<Func<T, bool>> filterExpression)
        {
            collection.DeleteMany(filterExpression);
        }

        /// <summary>
        /// Hard Delete
        /// </summary>
        /// <param name="filterExpression"></param>
        public virtual async Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression)
        {
            await collection.DeleteManyAsync(filterExpression);
        }
    }
}