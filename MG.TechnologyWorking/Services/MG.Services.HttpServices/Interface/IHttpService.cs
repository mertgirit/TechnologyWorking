using System.Threading.Tasks;
using System.Collections.Generic;

namespace MG.Services.HttpServices.Interface
{
    public interface IHttpService
    {
        Task<T> PostAsync<T>(string apiUrl, string apiMethod, object requestModel, string mediatype = "application/json", string token = "", string tokenType = "Bearer");

        Task<T> GetAsync<T>(string apiUrl, string apiMethod, string mediatype = "application/json", string token = "", string tokenType = "Bearer");

        Task<byte[]> GetAsync(string apiUrl, string apiMethod, string mediatype = "application/json", string token = "", string tokenType = "Bearer");

        bool DeleteAsync(string apiUrl, string apiMethod, string token, string tokenType = "Bearer");

        Task<T> MultipartFormDataPost<T>(string postUrl, Dictionary<string, object> postParameters);
    }
}