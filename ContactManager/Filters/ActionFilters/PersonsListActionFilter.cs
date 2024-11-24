using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace ContactManager.Filters.ActionFilters;

public class PersonsListActionFilter(ILogger<PersonsListActionFilter> logger) : IActionFilter
{
    private readonly ILogger<PersonsListActionFilter> _logger = logger;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("PersonListActionFilter onActionExecuting method");
        if (context.ActionArguments.TryGetValue("searchBy", out object? value))
        {
            string? searchBy = Convert.ToString(value);

            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchByOptions = new List<string>()
                {
                    nameof(PersonResponse.Name),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.Address),
                    nameof(PersonResponse.DateOfBirth),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.CountryId)
                };

                if (searchByOptions.Any(temp => temp == searchBy))
                {
                    _logger.LogInformation("SearchBy actually value is {searchBy}", searchBy);

                    context.ActionArguments["searchBy"] = nameof(PersonResponse.Name);

                    _logger.LogInformation("SearchBy updated value is {searchBy}", context.ActionArguments["searchBy"]);
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("PersonListActionFilter OnActionExecuted method");
    }
}