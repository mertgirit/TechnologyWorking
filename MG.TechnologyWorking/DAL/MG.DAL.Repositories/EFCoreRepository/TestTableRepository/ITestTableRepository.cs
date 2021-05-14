using System.Threading.Tasks;

namespace MG.DAL.Repositories.EFCoreRepository.TestTableRepository
{
    using MG.Models.DataModels;

    public interface ITestTableRepository : IRepository<TestTable>
    {
        Task<TestTable> GetByNameOrSomeThing(string name);
    }
}