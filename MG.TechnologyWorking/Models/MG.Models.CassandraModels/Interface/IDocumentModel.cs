using System;

namespace MG.Models.CassandraModels.Interface
{
    public interface IDocumentModel
    {
        Guid Id { get; set; }

        string CreateDate { get; }
    }
}