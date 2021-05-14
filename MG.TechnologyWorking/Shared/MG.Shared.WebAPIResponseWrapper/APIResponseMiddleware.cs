using MG.Shared.Enums;
using MG.Shared.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MG.Shared.WebAPIResponseWrapper
{
    public class APIResponseMiddleware
    {
        private readonly RequestDelegate Next;
        public APIResponseMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (IsSwagger(httpContext))
            {
                await Next(httpContext);
            }
            else
            {
                var originalBodyStream = httpContext.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    httpContext.Response.Body = responseBody;
                    try
                    {
                        await Next.Invoke(httpContext);

                        if (httpContext.Response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            var body = await FormatResponse(httpContext.Response);
                            await HandleSuccessRequestAsync(httpContext, body, httpContext.Response.StatusCode);
                        }
                        else
                        {
                            await HandleNotSuccessRequestAsync(httpContext, httpContext.Response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        await HandleExceptionAsync(httpContext, ex);
                    }
                    finally
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ApiError apiError = null;
            ApiResponse apiResponse = null;
            int code = 0;

            if (exception is ApiException)
            {
                var ex = exception as ApiException;
                apiError = new ApiError(ex.Message);
                apiError.ValidationErrors = ex.Errors;
                apiError.ReferenceErrorCode = ex.ReferenceErrorCode;
                apiError.ReferenceDocumentLink = ex.ReferenceDocumentLink;
                code = ex.StatusCode;
                context.Response.StatusCode = code;
            }
            else if (exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access");
                code = (int)HttpStatusCode.Unauthorized;
                context.Response.StatusCode = code;
            }
            else
            {
#if !DEBUG
            var msg = "An unhandled error occurred.";
            string stack = null;
#else
                var msg = exception.GetBaseException().Message;
                string stack = exception.StackTrace;
#endif

                apiError = new ApiError(msg);
                apiError.Details = stack;
                code = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = code;
            }

            context.Response.ContentType = "application/json";

            apiResponse = new ApiResponse(code, WebAPIResponseMessageEnum.Exception.GetDescription(), null, apiError);

            var json = JsonConvert.SerializeObject(apiResponse);

            return context.Response.WriteAsync(json);
        }

        private static Task HandleNotSuccessRequestAsync(HttpContext context, int code)
        {
            context.Response.ContentType = "application/json";

            ApiError apiError = null;
            ApiResponse apiResponse = null;

            if (code == (int)HttpStatusCode.NotFound)
            {
                apiError = new ApiError("The specified URI does not exist. Please verify and try again.");
            }
            else if (code == (int)HttpStatusCode.NoContent)
            {
                apiError = new ApiError("The specified URI does not contain any content.");
            }
            else
            {
                apiError = new ApiError("Your request cannot be processed. Please contact a support.");
            }

            apiResponse = new ApiResponse(code, WebAPIResponseMessageEnum.Failure.GetDescription(), null, apiError);
            context.Response.StatusCode = code;

            var json = JsonConvert.SerializeObject(apiResponse);

            return context.Response.WriteAsync(json);
        }

        private static Task HandleSuccessRequestAsync(HttpContext context, object body, int code)
        {
            context.Response.ContentType = "application/json";
            string jsonString, bodyText = string.Empty;
            ApiResponse apiResponse = null;


            if (!body.ToString().IsValidJson())
            {
                bodyText = JsonConvert.SerializeObject(body);
            }
            else
            {
                bodyText = body.ToString();
            }

            dynamic bodyContent = JsonConvert.DeserializeObject<dynamic>(bodyText);
            Type type;

            type = bodyContent?.GetType();

            if (type.Equals(typeof(Newtonsoft.Json.Linq.JObject)))
            {
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(bodyText);
                if (apiResponse.StatusCode != code)
                {
                    jsonString = JsonConvert.SerializeObject(apiResponse);
                }
                else if (apiResponse.Result != null)
                {
                    jsonString = JsonConvert.SerializeObject(apiResponse);
                }
                else
                {
                    apiResponse = new ApiResponse(code, WebAPIResponseMessageEnum.Success.GetDescription(), bodyContent, null);
                    jsonString = JsonConvert.SerializeObject(apiResponse);
                }
            }
            else
            {
                apiResponse = new ApiResponse(code, WebAPIResponseMessageEnum.Success.GetDescription(), bodyContent, null);
                jsonString = JsonConvert.SerializeObject(apiResponse);
            }

            return context.Response.WriteAsync(jsonString);
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var plainBodyText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return plainBodyText;
        }

        private bool IsSwagger(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/swagger");
        }
    }
}