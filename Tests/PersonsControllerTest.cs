using AutoFixture;
using ContactManager.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace Tests;

public class PersonsControllerTest
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly Mock<IPersonsService> _personsServiceMock;
    private readonly Mock<ICountriesService> _countriesServiceMock;
    private readonly Fixture _fixture;

    public PersonsControllerTest()
    {
        _fixture = new Fixture();
        _countriesServiceMock = new Mock<ICountriesService>();
        _personsServiceMock = new Mock<IPersonsService>();
        _countriesService = _countriesServiceMock.Object;
        _personsService = _personsServiceMock.Object;
    }

    #region Index

    [Fact]
    public async Task Index_ShouldReturnIndexViewWithPersonList()
    {
        List<PersonResponse> personResponses = _fixture.Create<List<PersonResponse>>();

        PersonsController personsController = new PersonsController(_personsService, _countriesService);

        _personsServiceMock.Setup(temp => temp.GetFilteredPerson(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(personResponses);
        _personsServiceMock.Setup(temp =>
                temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(),
                    It.IsAny<SortOrderOptions>()))
            .ReturnsAsync(personResponses);

        IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<SortOrderOptions>());

        ViewResult viewResult = Assert.IsType<ViewResult>(result);

        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
        viewResult.ViewData.Model.Should().Be(personResponses);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_IfModelErrors_ToReturnCreateView()
    {
        PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
        PersonResponse personResponse = _fixture.Create<PersonResponse>();
        List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

        _countriesServiceMock
            .Setup(temp => temp.GetAllCountries())
            .ReturnsAsync(countries);

        _personsServiceMock
            .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);

        PersonsController personsController = new PersonsController(_personsService, _countriesService);

        personsController.ModelState.AddModelError("PersonName", "Person Name can't be blank");

        IActionResult result = await personsController.Create(personAddRequest);
        ViewResult viewResult = Assert.IsType<ViewResult>(result);

        viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
        viewResult.ViewData.Model.Should().Be(personAddRequest);
    }


    [Fact]
    public async Task Create_IfNoModelErrors_ToReturnRedirectToIndex()
    {
        PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
        PersonResponse personResponse = _fixture.Create<PersonResponse>();
        List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

        _countriesServiceMock
            .Setup(temp => temp.GetAllCountries())
            .ReturnsAsync(countries);

        _personsServiceMock
            .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);

        PersonsController personsController = new PersonsController(_personsService, _countriesService);
        IActionResult result = await personsController.Create(personAddRequest);
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

        redirectResult.ActionName.Should().Be("Index");
    }

    #endregion
}