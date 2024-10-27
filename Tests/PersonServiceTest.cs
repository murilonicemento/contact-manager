using Entities;
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

    public PersonServiceTest(ITestOutputHelper testOutputHelper)
    {
        _personsService = new PersonsService();
        _countriesService = new CountriesService();
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    // When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public void AddPerson_NullPerson()
    {
        PersonAddRequest? personAddRequest = null;

        Assert.Throws<ArgumentNullException>(() => { _personsService.AddPerson(personAddRequest); });
    }

    // When we supply null value as person name, it should throw ArgumentException
    [Fact]
    public void AddPerson_PersonNameIsNull()
    {
        PersonAddRequest personAddRequest = new PersonAddRequest() { Name = null };

        Assert.Throws<ArgumentException>(() => { _personsService.AddPerson(personAddRequest); });
    }

    // When we supply proper person details, it should insert the person into the person list; and it should return object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public void AddPerson_ProperPersonDetails()
    {
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "Yeti",
            Email = "yeti@gmail.com",
            Address = "Xique Xique - BA",
            Gender = GenderOptions.Male,
            CountryId = Guid.NewGuid(),
            DateOfBirth = DateTime.Parse("2002-05-28"),
            ReceiveNewsLetters = true
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest: personAddRequest);
        List<PersonResponse> listOfPerson = _personsService.GetAllPersons();

        Assert.True(personResponse.Id != Guid.Empty);
        Assert.Contains(personResponse, listOfPerson);
    }

    #endregion

    #region GetPersonByPersonId

    // If we supply null as person id, it should return as response
    [Fact]
    public void GetPersonByPersonId_NullId()
    {
        PersonResponse? personResponse = _personsService.GetPersonByPersonId(null);

        Assert.Null(personResponse);
    }

    // If we supply a valid person id, it should return the valid person details as response
    [Fact]
    public void GetPersonByPersonId_ProperPersonDetails()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            Name = "Japan"
        };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "Yeti",
            Email = "yeti@gmail.com",
            Address = countryResponse.Name,
            Gender = GenderOptions.Male,
            CountryId = countryResponse.Id,
            DateOfBirth = DateTime.Parse("2002-05-28"),
            ReceiveNewsLetters = true
        };

        PersonResponse personResponseAddPerson = _personsService.AddPerson(personAddRequest: personAddRequest);

        PersonResponse? personResponseGetPersonId = _personsService.GetPersonByPersonId(personResponseAddPerson.Id);

        Assert.Equal(personResponseAddPerson, personResponseGetPersonId);
    }

    #endregion

    #region GetAllPerson

    // should return an empty list by default
    [Fact]
    public void GetAllPerson_EmptyByDefault()
    {
        List<PersonResponse> personResponses = _personsService.GetAllPersons();

        Assert.Empty(personResponses);
    }

    // first we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public void GetAllPerson_ProperPersonsDetails()
    {
        // first person
        List<PersonResponse> personResponses = new List<PersonResponse>();
        CountryAddRequest countryAddRequest = new CountryAddRequest() { Name = "Switzerland" };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "Yeti",
            Email = "yeti@gmail.com",
            Address = countryResponse.Name,
            Gender = GenderOptions.Male,
            CountryId = countryResponse.Id,
            DateOfBirth = DateTime.Parse("2002-05-28"),
            ReceiveNewsLetters = true
        };
        // second person
        CountryAddRequest countryAddRequest1 = new CountryAddRequest() { Name = "Brazil" };
        CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            Name = "Ronaldo",
            Email = "ronaldo@gmail.com",
            Address = countryResponse1.Name,
            Gender = GenderOptions.Male,
            CountryId = countryResponse1.Id,
            DateOfBirth = DateTime.Parse("1985-01-05"),
            ReceiveNewsLetters = false
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        PersonResponse personResponse1 = _personsService.AddPerson(personAddRequest1);

        personResponses.Add(personResponse);
        personResponses.Add(personResponse1);

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> personResponsesGetAll = _personsService.GetAllPersons();

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person in personResponsesGetAll)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        foreach (PersonResponse person in personResponses)
        {
            Assert.Contains(person, personResponsesGetAll);
        }
    }

    #endregion

    #region GetFilteredPersons

    // If the search text is empty search by is person name, it should return all persons
    [Fact]
    public void GetFilteredPersons_EmptySearchText()
    {
        // first person
        List<PersonResponse> personResponses = new List<PersonResponse>();
        CountryAddRequest countryAddRequest = new CountryAddRequest() { Name = "Switzerland" };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "Yeti",
            Email = "yeti@gmail.com",
            Address = countryResponse.Name,
            Gender = GenderOptions.Male,
            CountryId = countryResponse.Id,
            DateOfBirth = DateTime.Parse("2002-05-28"),
            ReceiveNewsLetters = true
        };
        // second person
        CountryAddRequest countryAddRequest1 = new CountryAddRequest() { Name = "Brazil" };
        CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            Name = "Ronaldo",
            Email = "ronaldo@gmail.com",
            Address = countryResponse1.Name,
            Gender = GenderOptions.Male,
            CountryId = countryResponse1.Id,
            DateOfBirth = DateTime.Parse("1985-01-05"),
            ReceiveNewsLetters = false
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        PersonResponse personResponse1 = _personsService.AddPerson(personAddRequest1);

        personResponses.Add(personResponse);
        personResponses.Add(personResponse1);

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> personResponsesSearch = _personsService.GetFilteredPerson(nameof(Person.Name), "");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person in personResponsesSearch)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        foreach (PersonResponse person in personResponses)
        {
            Assert.Contains(person, personResponsesSearch);
        }
    }

    // First we will add few persons; and then we will search based on person name with some string. It should return the matching persons
    [Fact]
    public void GetFilteredPersons_SearchByPersonName()
    {
        // first person
        List<PersonResponse> personResponses = new List<PersonResponse>();
        CountryAddRequest countryAddRequest = new CountryAddRequest() { Name = "Switzerland" };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "Mario",
            Email = "mario@gmail.com",
            Address = countryResponse.Name,
            Gender = GenderOptions.Male,
            CountryId = countryResponse.Id,
            DateOfBirth = DateTime.Parse("2002-05-28"),
            ReceiveNewsLetters = true
        };
        // second person
        CountryAddRequest countryAddRequest1 = new CountryAddRequest() { Name = "Brazil" };
        CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            Name = "Mary",
            Email = "mary@gmail.com",
            Address = countryResponse1.Name,
            Gender = GenderOptions.Male,
            CountryId = countryResponse1.Id,
            DateOfBirth = DateTime.Parse("1985-01-05"),
            ReceiveNewsLetters = false
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        PersonResponse personResponse1 = _personsService.AddPerson(personAddRequest1);

        personResponses.Add(personResponse);
        personResponses.Add(personResponse1);

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> personResponsesSearch = _personsService.GetFilteredPerson(nameof(Person.Name), "ma");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person in personResponsesSearch)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        foreach (PersonResponse person in personResponses)
        {
            if (person.Name == null) continue;

            if (person.Name.Contains("ma", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Contains(person, personResponsesSearch);
            }
        }
    }

    #endregion

    #region GetSortedPersons

    // When we sort based on PersonName in DESC order, it should return persons list in descending on PersonName
    [Fact]
    public void GetSortedPersons_SearchByPersonName()
    {
        // first person
        List<PersonResponse> personResponses = new List<PersonResponse>();
        CountryAddRequest countryAddRequest = new CountryAddRequest() { Name = "Switzerland" };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "Ronaldo",
            Email = "ronaldo@gmail.com",
            Address = countryResponse.Name,
            Gender = GenderOptions.Male,
            CountryId = countryResponse.Id,
            DateOfBirth = DateTime.Parse("2002-05-28"),
            ReceiveNewsLetters = true
        };
        // second person
        CountryAddRequest countryAddRequest1 = new CountryAddRequest() { Name = "Brazil" };
        CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            Name = "Mary",
            Email = "mary@gmail.com",
            Address = countryResponse1.Name,
            Gender = GenderOptions.Male,
            CountryId = countryResponse1.Id,
            DateOfBirth = DateTime.Parse("1985-01-05"),
            ReceiveNewsLetters = false
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        PersonResponse personResponse1 = _personsService.AddPerson(personAddRequest1);

        personResponses.Add(personResponse);
        personResponses.Add(personResponse1);

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> allPerson = _personsService.GetAllPersons();

        List<PersonResponse> personResponsesSearch =
            _personsService.GetSortedPersons(allPerson, nameof(Person.Name), SortOrderOptions.DESC);

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person in personResponsesSearch)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        personResponses = personResponses.OrderByDescending(person => person.Name).ToList();

        for (int i = 0; i < allPerson.Count; i++)
        {
            Assert.Equal(personResponsesSearch[i], personResponses[i]);
        }
    }

    #endregion

    #region UpdatePerson

    // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public void UpdatePerson_NullPerson()
    {
        PersonUpdateRequest? personUpdateRequest = null;

        Assert.Throws<ArgumentNullException>(() => { _personsService.UpdatePerson(personUpdateRequest); });
    }

    // When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public void UpdatePerson_InvalidPersonId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            Name = "Brazil"
        };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
        PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
        {
            Id = Guid.NewGuid(),
            Name = "Yeti",
            Email = "yeti@gmail.com",
            DateOfBirth = DateTime.Parse("2002-08-09"),
            Gender = GenderOptions.Male,
            CountryId = countryResponse.Id,
            Address = "Rua",
            ReceiveNewsLetters = true
        };

        Assert.Throws<ArgumentException>(() => { _personsService.UpdatePerson(personUpdateRequest); });
    }

    // When person name is null, it should throw ArgumentException
    [Fact]
    public void UpdatePerson_NullPersonName()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            Name = "Brazil"
        };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "Yeti",
            Email = "yeti@gmail.com",
            DateOfBirth = DateTime.Parse("2002-08-09"),
            Gender = GenderOptions.Male,
            CountryId = countryResponse.Id,
            Address = "Rua",
            ReceiveNewsLetters = true
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        personResponse.Name = null;
        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        Assert.Throws<ArgumentException>(() => { _personsService.UpdatePerson(personUpdateRequest); });
    }

    // First, add a new person and try update the same
    [Fact]
    public void UpdatePerson_PersonFullDetails()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            Name = "Brazil"
        };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "Yeti",
            Email = "yeti@gmail.com",
            DateOfBirth = DateTime.Parse("2002-08-09"),
            Gender = GenderOptions.Male,
            CountryId = countryResponse.Id,
            Address = "Rua",
            ReceiveNewsLetters = true
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        personUpdateRequest.Name = "Zé Gatinha";
        personUpdateRequest.Email = "lilCat@gmail.com";

        PersonResponse personResponseFromUpdate = _personsService.UpdatePerson(personUpdateRequest);
        PersonResponse? personResponseFromGet = _personsService.GetPersonByPersonId(personResponseFromUpdate.Id);

        Assert.Equal(personResponseFromGet, personResponseFromUpdate);
    }

    #endregion

    #region DeletePerson

    // If supply valid person id, it should return true
    [Fact]
    public void DeletePerson_ValidId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest() { Name = "Japan" };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest: countryAddRequest);
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "Yeti",
            Email = "yeti@gmail.com",
            DateOfBirth = DateTime.Parse("2002-08-09"),
            Gender = GenderOptions.Male,
            CountryId = countryResponse.Id,
            Address = "Rua",
            ReceiveNewsLetters = true
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        bool isDeleted = _personsService.DeletePerson(personResponse.Id);

        Assert.True(isDeleted);
    }

    // If supply invalid person id, it should return false
    [Fact]
    public void DeletePerson_InvalidId()
    {
        bool isDeleted = _personsService.DeletePerson(Guid.NewGuid());

        Assert.False(isDeleted);
    }

    #endregion
}