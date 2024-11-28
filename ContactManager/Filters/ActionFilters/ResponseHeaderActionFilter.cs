using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ActionFilters;

public class ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
    : IAsyncActionFilter, IOrderedFilter
{
    public int Order { get; set; } = order;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(ResponseHeaderActionFilter),
            nameof(OnActionExecutionAsync));

        await next();

        logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(ResponseHeaderActionFilter),
            nameof(OnActionExecutionAsync));

        context.HttpContext.Response.Headers[key] = value;
    }
}