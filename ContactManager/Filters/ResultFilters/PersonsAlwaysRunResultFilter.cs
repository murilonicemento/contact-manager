using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ResultFilters;

public class PersonsAlwaysRunResultFilter : IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Filters.OfType<SkipFilter>().Any()) return;
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}