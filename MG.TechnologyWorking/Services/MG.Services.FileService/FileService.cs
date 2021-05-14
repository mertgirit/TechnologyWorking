using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MG.Services.FileService
{
    using MG.Services.FileService.Interface;

    public class FileService : IFileService
    {
        public async Task<byte[]> GetFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found. Path: {filePath}");
            }

            return await File.ReadAllBytesAsync(filePath);
        }

        public byte[] GetFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found. Path: {filePath}");
            }

            return File.ReadAllBytes(filePath);
        }

        public async Task WriteFileAsync(byte[] data, string destinationPath)
        {
            await File.WriteAllBytesAsync(destinationPath, data);
            //TODO MG:
        }

        public async Task CheckDirectoryAsync(string path)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new System.Exception("path is empty");
                }

                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                catch (System.Exception ex)
                {
                    throw;
                }
            });
        }

        public async Task CheckDirectoriesAsync(List<string> paths)
        {
            await Task.Run(() =>
            {
                foreach (var path in paths)
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        throw new System.Exception("path is empty");
                    }

                    try
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        throw;
                    }
                }
            });
        }
    }
}