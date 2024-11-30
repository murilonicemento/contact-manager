using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.AuthorizationFilter;

public class TokenAuthorizationFilter : IAuthorizationFilter
{
    private readonly ILogger<TokenAuthorizationFilter> _logger;

    public TokenAuthorizationFilter(ILogger<TokenAuthorizationFilter> logger)
    {
        _logger = logger;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        _logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(TokenAuthorizationFilter),
            nameof(OnAuthorization));

        if (context.HttpContext.Request.Cookies.ContainsKey("Auth-Key") == false)
        {
            context.Result = new UnauthorizedResult();
        }

        if (context.HttpContext.Request.Cookies["Auth-Key"] != "A100")
        {
            context.Result = new UnauthorizedResult();
        }
    }
}