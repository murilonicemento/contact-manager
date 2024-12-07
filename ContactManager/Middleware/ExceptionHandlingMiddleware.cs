using Serilog;

namespace ContactManager.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger,
        IDiagnosticContext diagnosticContext)
    {
        _next = next;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            if (exception.InnerException != null)
            {
                _logger.LogError("{ExceptionType} {ExceptionMessage}", exception.InnerException.GetType().ToString(),
                    exception.InnerException.Message);
            }
            else
            {
                _logger.LogError("{ExceptionType} {ExceptionMessage}", exception.GetType().ToString(),
                    exception.Message);
            }

            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync("Error ocurred.");
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}