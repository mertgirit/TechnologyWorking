using System.Threading.Tasks;
using System.Collections.Generic;

namespace MG.Services.FileService.Interface
{
    public interface IFileService
    {
        Task WriteFileAsync(byte[] data, string destinationPath);

        Task<byte[]> GetFileAsync(string filePath);

        byte[] GetFile(string filePath);

        Task CheckDirectoryAsync(string path);

        Task CheckDirectoriesAsync(List<string> paths);
    }
}