using System.ComponentModel;

namespace MG.Shared.Enums
{
    public enum WebAPIResponseMessageEnum
    {
        [Description("Request successful.")]
        Success,

        [Description("Request responded with exceptions.")]
        Exception,

        [Description("Request denied.")]
        UnAuthorized,

        [Description("Request responded with validation error(s).")]
        ValidationError,

        [Description("Unable to process the request.")]
        Failure
    }
}