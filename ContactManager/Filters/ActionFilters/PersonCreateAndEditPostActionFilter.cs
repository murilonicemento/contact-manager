using ContactManager.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace ContactManager.Filters.ActionFilters;

public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
{
    private readonly ICountriesService _countriesService;

    public PersonCreateAndEditPostActionFilter(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is PersonsController { ModelState.IsValid: false } personsController)
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            personsController.ViewBag.Countries = countries.Select(temp =>
                new SelectListItem { Text = temp.Name, Value = temp.Id.ToString() });
            personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();

            context.Result =
                personsController.View("~/Views/Persons/Create.cshtml", context.ActionArguments
                    ["personRequest"]); // short circuits or skips the next action filter and action method
        }
        else
        {
            await next();
        }
    }
}