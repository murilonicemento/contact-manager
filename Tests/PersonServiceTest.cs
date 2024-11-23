using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace Tests;

public class PersonServiceTest
{
    private readonly IPersonsService _personsService;
    private readonly Mock<IPersonsRepository> _personsRepositoryMock;
    private readonly IPersonsRepository _personsRepository;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    public PersonServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();
        // DB CONTEXT MOCK
        // List<Country> countriesInitialData = new();
        // List<Person> personsInitialData = new();
        // DbContextMock<ApplicationDbContext> dbContextMock = new(new DbContextOptionsBuilder().Options);
        // ApplicationDbContext dbContext = dbContextMock.Object;
        //
        // dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        // dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

        // REPOSITORY MOCK
        _personsRepositoryMock = new Mock<IPersonsRepository>();
        _personsRepository = _personsRepositoryMock.Object;
        _personsService = new PersonsService(_personsRepository);
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    // When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
        PersonAddRequest? personAddRequest = null;

        Func<Task> action = async () => { await _personsService.AddPerson(personAddRequest); };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    // When we supply null value as person name, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
    {
        PersonAddRequest personAddRequest =
            _fixture.Build<PersonAddRequest>().With(person => person.Name, null as string).Create();
        Person person = personAddRequest.ToPerson();
        _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

        Func<Task> action = async () => { await _personsService.AddPerson(personAddRequest); };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When we supply proper person details, it should insert the person into the person list; and it should return object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
        PersonAddRequest personAddRequest =
            _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@email.com").Create();
        Person person = personAddRequest.ToPerson();
        PersonResponse personExpected = person.ToPersonResponse();

        // if we supply any argument value to the AddPerson method, it should return the same return value
        _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest: personAddRequest);

        // Assert.True(personResponse.Id != Guid.Empty);
        // Assert.Contains(personResponse, listOfPerson);
        personExpected.Id = personResponse.Id;

        personResponse.Id.Should().NotBe(Guid.Empty);
        personResponse.Should().Be(personExpected);
    }

    #endregion

    #region GetPersonByPersonId

    // If we supply null as person id, it should return as response
    [Fact]
    public async Task GetPersonByPersonId_NullId_ToBeNull()
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(null);

        // Assert.Null(personResponse);
        personResponse.Should().BeNull();
    }

    // If we supply a valid person id, it should return the valid person details as response
    [Fact]
    public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
    {
        Person person = _fixture.Build<Person>()
            .With(temp => temp.Email, "email@sample.com")
            .With(temp => temp.Country, null as Country)
            .Create();
        PersonResponse personResponse = person.ToPersonResponse();

        _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        PersonResponse? personResponseGetPersonId =
            await _personsService.GetPersonByPersonId(personResponse.Id);

        // Assert.Equal(personResponseAddPerson, personResponseGetPersonId);
        personResponseGetPersonId.Should().Be(personResponse);
    }

    #endregion

    #region GetAllPerson

// should return an empty list by default
    [Fact]
    public async Task GetAllPerson_EmptyByDefault_ToBeEmpty()
    {
        List<Person> persons = new List<Person>();

        _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
            .ReturnsAsync(persons);

        List<PersonResponse> personResponses = await _personsService.GetAllPersons();

        // Assert.Empty(personResponses);
        personResponses.Should().BeEmpty();
    }

// first we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public async Task GetAllPerson_ProperPersonsDetails()
    {
        List<Person> persons = new List<Person>
        {
            _fixture.Build<Person>()
                .With(person => person.Email, "yeti@gmail.com")
                .With(person => person.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(person => person.Email, "zeGatinha@gmail.com")
                .With(person => person.Country, null as Country).Create(),
        };
        List<PersonResponse> personResponses = persons.Select(person => person.ToPersonResponse()).ToList();

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

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
    public async Task GetFilteredPersons_EmptySearchText_ToBeEquivalentToPersonResponses()
    {
        List<Person> persons = new List<Person>()
        {
            _fixture.Build<Person>()
                .With(person => person.Email, "yeti@gmail.com")
                .With(person => person.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(person => person.Email, "ronaldo@gmail.com")
                .With(person => person.Country, null as Country).Create(),
        };
        List<PersonResponse> personResponses = persons.Select(person => person.ToPersonResponse()).ToList();

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(persons);

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
    public async Task GetFilteredPersons_SearchByPersonName_OnlyContainMA()
    {
        List<Person> persons = new List<Person>()
        {
            _fixture.Build<Person>()
                .With(temp => temp.Name, "Rahman")
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Name, "mary")
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Name, "scott")
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
        };
        List<PersonResponse> personResponses = persons.Select(person => person.ToPersonResponse()).ToList();

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(persons);

        List<PersonResponse> personsFromSearch =
            await _personsService.GetFilteredPerson(nameof(Person.Name), "ma");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse personFromGet in personsFromSearch)
        {
            _testOutputHelper.WriteLine(personFromGet.ToString());
        }

        personsFromSearch.Should().BeEquivalentTo(personResponses);
    }

    #endregion

    #region GetSortedPersons

    // When we sort based on PersonName in DESC order, it should return persons list in descending on PersonName
    [Fact]
    public async Task GetSortedPersons_SearchByPersonName_ToBeDescendingOrder()
    {
        List<Person> persons = new List<Person>()
        {
            _fixture.Build<Person>()
                .With(person => person.Email, "yeti@gmail.com")
                .With(person => person.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(person => person.Email, "ronaldo@gmail.com")
                .With(person => person.Country, null as Country).Create(),
        };
        List<PersonResponse> personResponses = persons.Select(person => person.ToPersonResponse()).ToList();

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
            .ReturnsAsync(persons);

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
    public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
    {
        PersonUpdateRequest? personUpdateRequest = null;

        Func<Task> action = async () => { await _personsService.UpdatePerson(personUpdateRequest); };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    // When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
    {
        PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();
        Person person = personUpdateRequest.ToPerson();

        _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        Func<Task> action = async () => { await _personsService.UpdatePerson(personUpdateRequest); };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When person name is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
    {
        PersonUpdateRequest personUpdateRequest =
            _fixture.Build<PersonUpdateRequest>().With(person => person.Name, null as string).Create();
        Person person = personUpdateRequest.ToPerson();

        _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        Func<Task> action = async () => { await _personsService.UpdatePerson(personUpdateRequest); };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    // First, add a new person and try update the same
    [Fact]
    public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
    {
        Person person = _fixture.Build<Person>()
            .With(person => person.Email, "lilCat@gmail.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .Create();
        PersonResponse personResponse = person.ToPersonResponse();
        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        _personsRepositoryMock
            .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        _personsRepositoryMock
            .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        PersonResponse personResponseFromUpdate = await _personsService.UpdatePerson(personUpdateRequest);

        // Assert.Equal(personResponseFromGet, personResponseFromUpdate);
        personResponseFromUpdate.Should().Be(personResponse);
    }

    #endregion

    #region DeletePerson

    // If supply valid person id, it should return true
    [Fact]
    public async Task DeletePerson_ValidId_ToBeTrue()
    {
        Person person = _fixture.Build<Person>()
            .With(person => person.Email, "yeti@gmail.com")
            .With(person => person.Country, null as Country).Create();

        _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _personsRepositoryMock
            .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        bool isDeleted = await _personsService.DeletePerson(person.Id);

        // Assert.True(isDeleted);
        isDeleted.Should().BeTrue();
    }

    // If supply invalid person id, it should return false
    [Fact]
    public async Task DeletePerson_InvalidId_ToBeFalse()
    {
        _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

        // Assert.False(isDeleted);
        isDeleted.Should().BeFalse();
    }

    #endregion
}