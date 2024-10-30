using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactManager.Controllers;

[Route("[controller]")]
public class PersonsController : Controller
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;

    public PersonsController(IPersonsService personsService, ICountriesService countriesService)
    {
        _personsService = personsService;
        _countriesService = countriesService;
    }

    [Route("[action]")]
    [Route("/")]
    public IActionResult Index(string searchBy, string? searchString,
        string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.Name), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryId), "Country" },
            { nameof(PersonResponse.Address), "Address" }
        };
        List<PersonResponse> persons = _personsService.GetFilteredPerson(searchBy, searchString);

        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;

        //Sort
        List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
        
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();

        return View(sortedPersons); 
    }


    [Route("[action]")]
    [HttpGet]
    public IActionResult Create()
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        
        ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.Name, Value = temp.Id.ToString() }
        );

        return View();
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            
            ViewBag.Countries = countries.Select(temp =>
                new SelectListItem() { Text = temp.Name, Value = temp.Id.ToString() });
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            
            return View();
        }

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

        return RedirectToAction("Index", "Persons");
    }
}