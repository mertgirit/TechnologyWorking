using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MG.Models.DataModels
{
    public class BaseEntity
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("DELETED")]
        public bool Deleted { get; set; }
    }
}