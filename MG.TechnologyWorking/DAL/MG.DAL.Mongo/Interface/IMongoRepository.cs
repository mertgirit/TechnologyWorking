using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace MG.DAL.Mongo.Interface
{
    using MG.Models.MongoModels.Interface;

    public interface IMongoRepository<T> where T : IDocumentModel
    {
        IEnumerable<T> FilterBy(
            Expression<Func<T, bool>> filterExpression);

        IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<T, bool>> filterExpression,
            Expression<Func<T, TProjected>> projectionExpression);

        T Find(Expression<Func<T, bool>> filterExpression);

        Task<T> FindAsync(Expression<Func<T, bool>> filterExpression);

        T FindById(string id);

        Task<T> FindByIdAsync(string id);

        void Insert(T document);

        Task InsertAsync(T document);

        Task InsertChunkAsync(T document);

        void InsertMany(ICollection<T> documents);

        Task InsertManyAsync(ICollection<T> documents);

        void Update(T document);

        Task UpdateAsync(T document);

        void Delete(Expression<Func<T, bool>> filterExpression);

        Task DeleteAsync(Expression<Func<T, bool>> filterExpression);

        void DeleteById(string id);

        Task DeleteByIdAsync(string id);

        void DeleteMany(Expression<Func<T, bool>> filterExpression);

        Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression);
    }
}