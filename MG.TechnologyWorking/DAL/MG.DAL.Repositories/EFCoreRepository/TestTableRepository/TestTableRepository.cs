using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MG.DAL.Repositories.EFCoreRepository.TestTableRepository
{
    using MG.Models.DataModels;
    using MG.DAL.EntityFrameworkCore;

    public class TestTableRepository : MGRepository<TestTable>, ITestTableRepository
    {
        private readonly MGContext context;
        public TestTableRepository(MGContext context)
            : base(context)
        {
            this.context = context;
        }

        public async Task<TestTable> GetByNameOrSomeThing(string name)
        {
            try
            {
                return await context.TestTables.Where(x => x.TestData.Equals(name)).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't retrieve count", ex);
            }
        }
    }
}