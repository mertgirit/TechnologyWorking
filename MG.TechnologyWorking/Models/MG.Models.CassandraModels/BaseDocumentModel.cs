using System;

namespace MG.Models.CassandraModels
{
    using MG.Models.CassandraModels.Interface;

    public abstract class BaseDocumentModel : IDocumentModel
    {
        public Guid Id { get; set; }

        public string CreateDate { get; set; }
    }
}