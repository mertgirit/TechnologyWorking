using Newtonsoft.Json;

namespace MG.Shared.WebAPIResponseWrapper
{
    public class ValidationError
    {
        /// <summary>
        /// Bu attribute, field alanının null olması durumunda serialized edilmemesini sağlar.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }
        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
}