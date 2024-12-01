using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ActionFilters;

public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
{
    public bool IsReusable { get; } = false;
    private readonly string _key;
    private readonly string _value;
    private readonly int _order;

    public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
    {
        _key = key;
        _value = value;
        _order = order;
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ResponseHeaderActionFilter filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();

        filter.Order = _order;
        filter.Key = _key;
        filter.Value = _value;

        return filter;
    }
}

public class ResponseHeaderActionFilter
    : IAsyncActionFilter, IOrderedFilter
{
    public int Order { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    private readonly ILogger<ResponseHeaderActionFilter> _logger;

    public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _logger.LogInformation("Before logic - ResponseHeaderActionFilter");
        await next();

        context.HttpContext.Response.Headers[Key] = Value;
    }
}