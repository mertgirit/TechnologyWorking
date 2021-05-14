using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MG.Test.EFCoreTest
{
    using MG.Models.DataModels;
    using MG.DAL.Repositories;
    using MG.DAL.EntityFrameworkCore;
    using MG.DAL.Repositories.EFCoreRepository.TestTableRepository;
    using MG.DAL.Repositories.EFCoreRepository;

    class Program
    {
        static ITestTableRepository TestTableRepository;
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<MGContext>(c => c.UseInMemoryDatabase("Test"))
                .AddScoped(typeof(IRepository<>), typeof(MGRepository<>))
                .AddScoped<ITestTableRepository, TestTableRepository>()
                .BuildServiceProvider();

            TestTableRepository = serviceProvider.GetService<ITestTableRepository>();

            TestTable testTable = new TestTable() { CreateDate = DateTime.Now, Deleted = false, Id = 1, TestData = "Test1" };
            var entity = await AddTestTableEntity(testTable);

            var getEntity = await GetTestTableEntityById(testTable.Id);

            getEntity.TestData = "Updated";
            var updatedEntity = await UpdateTestTableEntity(getEntity);

            getEntity = await GetTestTableEntityById(testTable.Id);

            testTable.Id = 2;
            entity = await AddTestTableEntity(testTable);

            var getEntityList = await GetTestTableEntityList();

            var deletedEntity = await DeleteTestTableEntity(testTable);

            getEntityList = await GetTestTableEntityList();
        }

        static async Task<TestTable> AddTestTableEntity(TestTable testTable)
        {
            return await TestTableRepository.AddAsync(testTable);
        }

        static async Task<bool> AddBulkTestTableEntityList(List<TestTable> testTable)
        {
            return await TestTableRepository.BulkInsertAsync(testTable);
        }

        static async Task<TestTable> UpdateTestTableEntity(TestTable testTable)
        {
            return await TestTableRepository.UpdateAsync(testTable);
        }

        static async Task<TestTable> DeleteTestTableEntity(TestTable testTable)
        {
            return await TestTableRepository.DeleteAsync(testTable.Id);
        }

        static async Task<TestTable> GetTestTableEntityById(int id)
        {
            return await TestTableRepository.GetByIdAsync(id);
        }

        static async Task<IEnumerable<TestTable>> GetTestTableEntityList()
        {
            return await TestTableRepository.GetAllAsync();
        }
    }
}