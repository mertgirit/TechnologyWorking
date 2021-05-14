using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MG.Services.HttpServices
{
    using MG.Services.HttpServices.Models;
    using MG.Services.HttpServices.Interface;

    public class HttpService : IHttpService
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        private readonly ILogger<HttpService> logger;
        public HttpService(ILogger<HttpService> logger)
        {
            this.logger = logger;
        }

        public async Task<T> PostAsync<T>(string apiUrl, string apiMethod, object requestModel, string mediatype = "application/json", string token = "", string tokenType = "Bearer")
        {
            using var httpClient = new HttpClient();
            try
            {
                httpClient.BaseAddress = new Uri(apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(mediatype));

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);
                }

                HttpResponseMessage response = await httpClient.PostAsJsonAsync(apiMethod, requestModel);
                response.EnsureSuccessStatusCode();

                var jsonResult = await response.Content.ReadAsAsync<T>();

                return jsonResult;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string apiUrl, string apiMethod, string mediatype = "application/json", string token = "", string tokenType = "Bearer")
        {
            try
            {
                using var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(apiUrl)
                };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(mediatype));

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);
                }

                HttpResponseMessage response = await httpClient.GetAsync(apiMethod);

                response.EnsureSuccessStatusCode();

                var jsonResult = await response.Content.ReadAsAsync<T>();

                return jsonResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<byte[]> GetAsync(string apiUrl, string apiMethod, string mediatype = "application/json", string token = "", string tokenType = "Bearer")
        {
            try
            {
                using var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(apiUrl)
                };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(mediatype));

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);
                }

                httpClient.Timeout = TimeSpan.FromMinutes(30);

                HttpResponseMessage response = await httpClient.GetAsync(apiMethod);

                response.EnsureSuccessStatusCode();

                byte[] bytearrayresult = await response.Content.ReadAsByteArrayAsync();

                return bytearrayresult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteAsync(string apiUrl, string apiMethod, string token, string tokenType = "Bearer")
        {
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);

            HttpResponseMessage response = httpClient.DeleteAsync(apiMethod).GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

        public async Task<T> MultipartFormDataPost<T>(string postUrl, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = await GetMultipartFormData(postParameters, formDataBoundary);

            return await PostForm<T>(postUrl, contentType, formData);
        }

        private async Task<T> PostForm<T>(string postUrl, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            request.Method = "POST";
            request.ContentType = contentType;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            // request.PreAuthenticate = true;
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            // request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("username" + ":" + "password")));

            using (Stream requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(formData, 0, formData.Length);
                requestStream.Close();
            }

            var response = await request.GetResponseAsync() as HttpWebResponse;

            StreamReader responseReader = new StreamReader(response.GetResponseStream());

            string fullResponse = await responseReader.ReadToEndAsync();
            response.Close();
            return JsonConvert.DeserializeObject<T>(fullResponse);
        }

        private async Task<byte[]> GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (needsCLRF)
                {
                    await formDataStream.WriteAsync(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));
                }

                needsCLRF = true;

                if (param.Value is FileParameterModel)
                {
                    FileParameterModel fileToUpload = (FileParameterModel)param.Value;

                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    await formDataStream.WriteAsync(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    await formDataStream.WriteAsync(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    await formDataStream.WriteAsync(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            string footer = "\r\n--" + boundary + "--\r\n";
            await formDataStream.WriteAsync(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            await formDataStream.ReadAsync(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }
}