using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactManager.Controllers;

public class PersonsController : Controller
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;

    public PersonsController(IPersonsService personsService, ICountriesService countriesService)
    {
        _personsService = personsService;
        _countriesService = countriesService;
    }

    [Route("/")]
    [Route("/persons/index")]
    public IActionResult Index(string searchBy, string searchString,
        SortOrderOptions sortOrder = SortOrderOptions.ASC,
        string sortBy = nameof(PersonResponse.Name))
    {
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            {
                nameof(PersonResponse.Name), "Person Name"
            },
            {
                nameof(PersonResponse.Email), "Email"
            },
            {
                nameof(PersonResponse.DateOfBirth), "Date of Birth"
            },
            {
                nameof(PersonResponse.Gender), "Gender"
            },
            {
                nameof(PersonResponse.Address), "Address"
            },
            {
                nameof(PersonResponse.CountryId), "Country"
            }
        };

        List<PersonResponse> persons = _personsService.GetFilteredPerson(searchBy, searchString);

        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;

        List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);

        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();

        return View(sortedPersons);
    }

    [HttpPost]
    [Route("persons/create")]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();

            ViewBag.Countries = countries;
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            return View();
        }

        //call the service method
        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

        //navigate to Index() action method (it makes another get request to "persons/index"
        return RedirectToAction("Index", "Persons");
    }
}