using Newtonsoft.Json;

namespace RaceCalendar.Api.Responses.Base;

public class ApiResponse
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; }

    [JsonProperty("message")]
    public string Message { get; }

    public ApiResponse(int statusCode, string message = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetMessageForStatusCode(statusCode);
    }

    private static string GetMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            404 => "The requested resource is not found",
            500 => "Internal server error occured",
            512 => "The resource already exists",
            _ => statusCode.ToString(),
        };
    }
}
