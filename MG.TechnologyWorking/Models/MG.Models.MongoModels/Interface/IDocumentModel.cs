using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MG.Models.MongoModels.Interface
{
    public interface IDocumentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        ObjectId Id { get; set; }

        DateTime CreateDate { get; }

        public byte[] Data { get; set; }
    }
}