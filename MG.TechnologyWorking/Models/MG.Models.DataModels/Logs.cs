using System.ComponentModel.DataAnnotations.Schema;

namespace MG.Models.DataModels
{
    public class Logs : BaseEntity
    {
        [Column("LOGLEVEL")]
        public string LogLevel { get; set; }

        [Column("MESSAGE")]
        public string Message { get; set; }

        [Column("ERRORMESSAGE")]
        public string ErrorMessage { get; set; }

        [Column("STACKTRACE")]
        public string StackTrace { get; set; }

        [Column("APPLICATION")]
        public string Application { get; set; }

        [Column("APPLICATIONSERVICE")]
        public string ApplicationService { get; set; }
    }
}