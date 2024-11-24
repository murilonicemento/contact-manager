using ContactManager.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace ContactManager.Filters.ActionFilters;

public class PersonsListActionFilter(ILogger<PersonsListActionFilter> logger) : IActionFilter
{
    private readonly ILogger<PersonsListActionFilter> _logger = logger;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items["arguments"] = context.ActionArguments;
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

        PersonsController personsController = (PersonsController)context.Controller;
        IDictionary<string, object?>?
            parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

        if (parameters != null)
        {
            if (parameters.TryGetValue("searchBy", out object? searchValue))
            {
                personsController.ViewData["CurrentSearchBy"] = searchValue?.ToString();
            }

            if (parameters.TryGetValue("searchString", out object? stringValue))
            {
                personsController.ViewData["CurrentSearchString"] = stringValue?.ToString();
            }

            if (parameters.TryGetValue("sortBy", out object? sortValue))
            {
                personsController.ViewData["CurrentSortBy"] = sortValue?.ToString();
            }

            if (parameters.TryGetValue("sortOrder", out object? sortOrderValue))
            {
                personsController.ViewData["CurrentSortOrder"] = sortOrderValue?.ToString();
            }
        }

        personsController.ViewData["SearchFields"] = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.Name), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryId), "Country" },
            { nameof(PersonResponse.Address), "Address" }
        };
    }
}