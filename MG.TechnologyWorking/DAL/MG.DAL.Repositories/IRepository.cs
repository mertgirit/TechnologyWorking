using System.Threading.Tasks;
using System.Collections.Generic;

namespace MG.DAL.Repositories
{
    public interface IRepository<T> where T : class, new()
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<T> AddAsync(T entity);

        Task<T> UpdateAsync(T entity);

        Task<T> DeleteAsync(int id);

        Task<bool> BulkInsertAsync(List<T> entities);

        Task<bool> BulkDeleteAsync(List<T> entities);
    }
}