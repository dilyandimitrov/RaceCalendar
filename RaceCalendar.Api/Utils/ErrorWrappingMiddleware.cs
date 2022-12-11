using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RaceCalendar.Api.Responses.Base;
using System.Data.SqlClient;

namespace RaceCalendar.Api.Utils;

public class ErrorWrappingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorWrappingMiddleware> _logger;

    public ErrorWrappingMiddleware(RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger)
    {
        _next = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, ex.Message);

            var sqlException = ex.InnerException as SqlException;

            if (ex.InnerException != null && sqlException != null && IsUniqueKeyViolation(sqlException))
            {
                _logger.LogError(sqlException, sqlException.Message);

                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(ConvertApiResponseToJson(context, 512));
            }

            context.Response.StatusCode = 500;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = 500;

            await context.Response.WriteAsync(ConvertApiResponseToJson(context, message: ex.Message));
        }

        if (!context.Response.HasStarted)
        {
            await context.Response.WriteAsync(ConvertApiResponseToJson(context));
        }
    }

    private static bool IsUniqueKeyViolation(SqlException ex)
    {
        return ex.Errors.Cast<SqlError>().Any(e => e.Class == 14 && (e.Number == 2601));
    }

    private static string ConvertApiResponseToJson(HttpContext context, int? statusCode = null, string message = null)
    {
        context.Response.ContentType = "application/json";
        var response = new ApiResponse(statusCode ?? context.Response.StatusCode, message);
        return JsonConvert.SerializeObject(response);
    }
}
