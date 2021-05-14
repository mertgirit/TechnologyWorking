using Microsoft.EntityFrameworkCore;

namespace MG.DAL.EntityFrameworkCore
{
    using MG.Models.DataModels;

    public class MGContext : DbContext
    {
        public MGContext()
        {
        }

        public MGContext(DbContextOptions<MGContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TestTable> TestTables { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}