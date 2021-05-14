using System;
using System.Linq;
using EFCore.BulkExtensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MG.DAL.Repositories.EFCoreRepository
{
    using MG.Models.DataModels;
    using MG.DAL.EntityFrameworkCore;

    public class MGRepository<T> : IRepository<T> where T : BaseEntity, new()
    {
        private readonly MGContext context;

        public MGRepository(MGContext context)
        {
            this.context = context;

            //Context objesi thread-safe değil, 
            //bu yüzden paralel işlemler yapılacaksa *Database.AutoTransactionsEnabled* property değeri false yapılıp
            //transaction ı kendiniz yönetmelisiniz. Default değer true.
            //context.Database.AutoTransactionsEnabled = false;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't retrieve entities", ex);
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await context.Set<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(DeleteAsync)} could not find entity by id: {id}", ex);
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(entity)} entity must not be null");
            }

            try
            {
                await context.AddAsync(entity);
                await context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"AgingRepo.AddAsync Error: {nameof(entity)} could not be saved", ex);
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateAsync)} entity must not be null");
            }

            try
            {
                context.Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated", ex);
            }
        }

        public async Task<T> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);

                entity.Deleted = true;
                return await UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(id)} could not be deleted", ex);
            }
        }

        public async Task<bool> BulkInsertAsync(List<T> entities)
        {
            if (entities != null && entities.Count > 0)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await context.BulkInsertAsync(entities);
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception($"{nameof(entities)} could not be saved", ex);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException($"{nameof(BulkInsertAsync)} entity must not be null");
            }
        }

        public async Task<bool> BulkDeleteAsync(List<T> entities)
        {
            if (entities != null && entities.Count > 0)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await context.BulkDeleteAsync(entities);
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception($"{nameof(entities)} could not be deleted", ex);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException($"{nameof(BulkDeleteAsync)} entity must not be null");
            }
        }
    }
}