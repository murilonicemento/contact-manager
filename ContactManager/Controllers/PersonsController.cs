using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace ContactManager.Controllers;

public class PersonsController : Controller
{
    private readonly IPersonsService _personsService;

    public PersonsController(IPersonsService personsService)
    {
        _personsService = personsService;
    }

    [Route("/")]
    [Route("/persons/index")]
    public IActionResult Index(string searchBy, string searchString)
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

        return View(persons);
    }
}