using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ActionFilters;

public class ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value)
    : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter),
            nameof(OnActionExecuting));
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter),
            nameof(OnActionExecuted));

        context.HttpContext.Response.Headers[key] = value;
    }
}