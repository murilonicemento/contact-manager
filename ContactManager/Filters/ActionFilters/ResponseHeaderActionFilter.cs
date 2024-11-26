using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ActionFilters;

public class ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
    : IActionFilter, IOrderedFilter
{
    public int Order { get; set; } = order;

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