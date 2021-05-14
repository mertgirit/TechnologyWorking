using System;
using MongoDB.Bson;

namespace MG.Models.MongoModels
{
    using MG.Models.MongoModels.Interface;
    public abstract class BaseDocumentModel : IDocumentModel
    {
        public ObjectId Id { get; set; }

        public DateTime CreateDate => Id.CreationTime;

        public byte[] Data { get; set; }
    }
}