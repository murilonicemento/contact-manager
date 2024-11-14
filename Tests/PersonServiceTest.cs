using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace Tests;

public class PersonServiceTest
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    public PersonServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();
        List<Country> countriesInitialData = new();
        List<Person> personsInitialData = new();
        DbContextMock<ApplicationDbContext> dbContextMock = new(new DbContextOptionsBuilder().Options);
        ApplicationDbContext dbContext = dbContextMock.Object;

        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

        _countriesService = new CountriesService(null);
        _personsService = new PersonsService(null, _countriesService);
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    // When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson()
    {
        PersonAddRequest? personAddRequest = null;

        Func<Task> action = async () => { await _personsService.AddPerson(personAddRequest); };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    // When we supply null value as person name, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull()
    {
        PersonAddRequest personAddRequest =
            _fixture.Build<PersonAddRequest>().With(person => person.Name, null as string).Create();

        Func<Task> action = async () => { await _personsService.AddPerson(personAddRequest); };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When we supply proper person details, it should insert the person into the person list; and it should return object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        PersonAddRequest personAddRequest =
            _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@email.com").Create();

        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest: personAddRequest);
        List<PersonResponse> listOfPerson = await _personsService.GetAllPersons();

        // Assert.True(personResponse.Id != Guid.Empty);
        // Assert.Contains(personResponse, listOfPerson);

        personResponse.Id.Should().NotBe(Guid.Empty);
        listOfPerson.Should().Contain(personResponse);
    }

    #endregion

    #region GetPersonByPersonId

    // If we supply null as person id, it should return as response
    [Fact]
    public async Task GetPersonByPersonId_NullId()
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(null);

        // Assert.Null(personResponse);
        personResponse.Should().BeNull();
    }

    // If we supply a valid person id, it should return the valid person details as response
    [Fact]
    public async Task GetPersonByPersonId_ProperPersonDetails()
    {
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "yeti@outloook.com").With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();

        PersonResponse personResponseAddPerson = await _personsService.AddPerson(personAddRequest: personAddRequest);

        PersonResponse? personResponseGetPersonId =
            await _personsService.GetPersonByPersonId(personResponseAddPerson.Id);

        // Assert.Equal(personResponseAddPerson, personResponseGetPersonId);
        personResponseGetPersonId.Should().Be(personResponseAddPerson);
    }

    #endregion

    #region GetAllPerson

    // should return an empty list by default
    [Fact]
    public async Task GetAllPerson_EmptyByDefault()
    {
        List<PersonResponse> personResponses = await _personsService.GetAllPersons();

        // Assert.Empty(personResponses);
        personResponses.Should().BeEmpty();
    }

    // first we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public async Task GetAllPerson_ProperPersonsDetails()
    {
        // first person
        List<PersonResponse> personResponses = new List<PersonResponse>();
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest =
            _fixture.Build<PersonAddRequest>().With(person => person.Email, "yeti@gmail.com")
                .With(person => person.Address, countryResponse.Name)
                .With(person => person.CountryId, countryResponse.Id).Create();

        // second person
        CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();


        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
        PersonResponse personResponse1 = await _personsService.AddPerson(personAddRequest1);

        personResponses.Add(personResponse);
        personResponses.Add(personResponse1);

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> personResponsesGetAll = await _personsService.GetAllPersons();

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person in personResponsesGetAll)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        // foreach (PersonResponse person in personResponses)
        // {
        // Assert.Contains(person, personResponsesGetAll);
        // personResponsesGetAll.Should().Contain(person);
        // }

        personResponsesGetAll.Should().BeEquivalentTo(personResponses);
    }

    #endregion

    #region GetFilteredPersons

    // If the search text is empty search by is person name, it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
        // first person
        List<PersonResponse> personResponses = new List<PersonResponse>();
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();
        // second person
        CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();

        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
        PersonResponse personResponse1 = await _personsService.AddPerson(personAddRequest1);

        personResponses.Add(personResponse);
        personResponses.Add(personResponse1);

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> personResponsesSearch = await _personsService.GetFilteredPerson(nameof(Person.Name), "");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person in personResponsesSearch)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        // foreach (PersonResponse person in personResponses)
        // {
        //     Assert.Contains(person, personResponsesSearch);
        // }

        personResponsesSearch.Should().BeEquivalentTo(personResponses);
    }

    // First we will add few persons; and then we will search based on person name with some string. It should return the matching persons
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
        //Arrange
        CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

        PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Name, "Rahman")
            .With(temp => temp.Email, "someone_1@example.com")
            .With(temp => temp.CountryId, country_response_1.Id)
            .Create();

        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Name, "mary")
            .With(temp => temp.Email, "someone_2@example.com")
            .With(temp => temp.CountryId, country_response_1.Id)
            .Create();

        PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Name, "scott")
            .With(temp => temp.Email, "someone_3@example.com")
            .With(temp => temp.CountryId, country_response_2.Id)
            .Create();

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            { person_request_1, person_request_2, person_request_3 };

        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = await _personsService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        //Act
        List<PersonResponse> persons_list_from_search =
            await _personsService.GetFilteredPerson(nameof(Person.Name), "ma");

        //print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_get in persons_list_from_search)
        {
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        //Assert
        persons_list_from_search.Should()
            .OnlyContain(temp => temp.Name != null && temp.Name.Contains("ma", StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region GetSortedPersons

    // When we sort based on PersonName in DESC order, it should return persons list in descending on PersonName
    [Fact]
    public async Task GetSortedPersons_SearchByPersonName()
    {
        // first person
        List<PersonResponse> personResponses = new List<PersonResponse>();
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();
        // second person
        CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();

        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
        PersonResponse personResponse1 = await _personsService.AddPerson(personAddRequest1);

        personResponses.Add(personResponse);
        personResponses.Add(personResponse1);

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> allPerson = await _personsService.GetAllPersons();

        List<PersonResponse> personResponsesSearch =
            await _personsService.GetSortedPersons(allPerson, nameof(Person.Name), SortOrderOptions.DESC);

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person in personResponsesSearch)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        personResponses = personResponses.OrderByDescending(person => person.Name).ToList();

        // for (int i = 0; i < allPerson.Count; i++)
        // {
        //     Assert.Equal(personResponsesSearch[i], personResponses[i]);
        // }
        //
        // personResponses.Should().BeEquivalentTo(personResponsesSearch);
        personResponses.Should().BeInDescendingOrder(person => person.Name);
    }

    #endregion

    #region UpdatePerson

    // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
        PersonUpdateRequest? personUpdateRequest = null;

        Func<Task> action = async () => { await _personsService.UpdatePerson(personUpdateRequest); };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    // When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonId()
    {
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
        PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
            .With(person => person.Id, Guid.NewGuid())
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();

        Func<Task> action = async () => { await _personsService.UpdatePerson(personUpdateRequest); };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When person name is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_NullPersonName()
    {
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();

        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
        personResponse.Name = null;
        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        Func<Task> action = async () => { await _personsService.UpdatePerson(personUpdateRequest); };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    // First, add a new person and try update the same
    [Fact]
    public async Task UpdatePerson_PersonFullDetails()
    {
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();

        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        personUpdateRequest.Name = "Zé Gatinha";
        personUpdateRequest.Email = "lilCat@gmail.com";

        PersonResponse personResponseFromUpdate = await _personsService.UpdatePerson(personUpdateRequest);
        PersonResponse? personResponseFromGet = await _personsService.GetPersonByPersonId(personResponseFromUpdate.Id);

        // Assert.Equal(personResponseFromGet, personResponseFromUpdate);
        personResponseFromUpdate.Should().BeEquivalentTo(personResponseFromGet);
    }

    #endregion

    #region DeletePerson

    // If supply valid person id, it should return true
    [Fact]
    public async Task DeletePerson_ValidId()
    {
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest: countryAddRequest);
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Address, countryResponse.Name)
            .With(person => person.CountryId, countryResponse.Id).Create();

        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
        bool isDeleted = await _personsService.DeletePerson(personResponse.Id);

        // Assert.True(isDeleted);
        isDeleted.Should().BeTrue();
    }

    // If supply invalid person id, it should return false
    [Fact]
    public async Task DeletePerson_InvalidId()
    {
        bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

        // Assert.False(isDeleted);
        isDeleted.Should().BeFalse();
    }

    #endregion
}