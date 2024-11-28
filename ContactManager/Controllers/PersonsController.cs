using ContactManager.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactManager.Controllers;

[Route("[controller]")]
[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments =
[
    "My-Key-Controller", "My-Value-Controller", 3
], Order = 3)]
public class PersonsController : Controller
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(IPersonsService personsService, ICountriesService countriesService,
        ILogger<PersonsController> logger)
    {
        _personsService = personsService;
        _countriesService = countriesService;
        _logger = logger;
    }

    [Route("[action]")]
    [Route("/")]
    [TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments =
    [
        "X-Custom-Key", "Custom-Value", 1
    ], Order = 1)]
    public async Task<IActionResult> Index(string searchBy, string? searchString,
        string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        _logger.LogInformation("Index action method in PersonsController");
        _logger.LogDebug(
            $"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

        List<PersonResponse> persons = await _personsService.GetFilteredPerson(searchBy, searchString);
        List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);

        return View(sortedPersons);
    }


    [Route("[action]")]
    [HttpGet]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments =
    [
        "My-Key", "My-Value"
    ], Order = 4)]
    public async Task<IActionResult> Create()
    {
        List<CountryResponse> countries = await _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.Name, Value = temp.Id.ToString() }
        );

        return View();
    }

    [HttpPost]
    [Route("[action]")]
    [TypeFilter(typeof(PersonCreateAndEditPostActionFilter), Order = 4)]
    public async Task<IActionResult> Create(PersonAddRequest personRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            ViewBag.Countries = countries.Select(temp =>
                new SelectListItem() { Text = temp.Name, Value = temp.Id.ToString() });
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            return View(personRequest);
        }

        await _personsService.AddPerson(personRequest);

        return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("[action]/{personId}")]
    public async Task<IActionResult> Edit(Guid personId)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(personId);

        if (personResponse is null) return RedirectToAction("Index");

        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        return View(personUpdateRequest);
    }

    [HttpPost]
    [Route("[action]/{personId}")]
    public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(personRequest.Id);

        if (personResponse is null) return RedirectToAction("Index");
        if (ModelState.IsValid)
        {
            await _personsService.UpdatePerson(personRequest);

            return RedirectToAction("Index");
        }

        List<CountryResponse> countries = await _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.Name, Value = temp.Id.ToString() });
        ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

        return View();
    }

    [HttpGet]
    [Route("[action]/{personId}")]
    public async Task<IActionResult> Delete(Guid? personId)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(personId);

        if (personResponse is null) return RedirectToAction("Index");

        return View(personResponse);
    }

    [HttpPost]
    [Route("[action]/{personId}")]
    public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(personUpdateRequest.Id);

        if (personResponse is null) RedirectToAction("Index");

        await _personsService.DeletePerson(personUpdateRequest.Id);

        return RedirectToAction("Index");
    }

    [Route("PersonsPDF")]
    public async Task<IActionResult> PersonsPDF()
    {
        List<PersonResponse> persons = await _personsService.GetAllPersons();

        return new ViewAsPdf("PersonsPDF", persons, ViewData)
        {
            PageMargins = new Margins
            {
                Top = 20, Right = 20, Bottom = 20, Left = 20
            },
            PageOrientation = Orientation.Landscape
        };
    }

    [Route("PersonsCSV")]
    public async Task<IActionResult> PersonsCSV()
    {
        MemoryStream memoryStream = await _personsService.GetPersonsCSV();

        return File(memoryStream, "application/octet-stream", "persons.csv");
    }

    [Route("PersonsExcel")]
    public async Task<IActionResult> PersonsExcel()
    {
        MemoryStream memoryStream = await _personsService.GetPersonExcel();

        return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
    }
}