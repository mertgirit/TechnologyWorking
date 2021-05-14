namespace MG.Models.CassandraModels
{
    using Cassandra.Mapping;

    [TableName("filedocument")]
    public class FileDocumentModel : BaseDocumentModel
    {
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string ExternalFileName { get; set; }
    }
}