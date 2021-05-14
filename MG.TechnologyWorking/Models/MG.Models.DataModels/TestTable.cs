using System.ComponentModel.DataAnnotations.Schema;

namespace MG.Models.DataModels
{
    public class TestTable : BaseEntity
    {
        [Column("TEST_DATA")]
        public string TestData { get; set; }
    }
}