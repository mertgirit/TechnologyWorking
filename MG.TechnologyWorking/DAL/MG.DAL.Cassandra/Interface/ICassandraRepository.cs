using MG.Models.CassandraModels.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MG.DAL.Cassandra.Interface
{
    public interface ICassandraRepository<T> where T : IDocumentModel, new()
    {
        Task<T> FindByIdAsync(Guid id);
        Task<T> InsertAsync(T model);
        void Insert(T model);
    }
}
