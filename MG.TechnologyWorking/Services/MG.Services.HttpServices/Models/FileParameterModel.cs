namespace MG.Services.HttpServices.Models
{
    public class FileParameterModel
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameterModel(byte[] file) : this(file, null) { }
        public FileParameterModel(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameterModel(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }
}