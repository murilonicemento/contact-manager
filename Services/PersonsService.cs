using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    private readonly List<Person> _persons;
    private readonly ICountriesService _countriesService;

    public PersonsService(bool initialize = true)
    {
        _persons = new List<Person>();
        _countriesService = new CountriesService();

        if (initialize)
        {
            _persons.AddRange(new List<Person>()
            {
                new Person()
                {
                    Id = Guid.NewGuid(),
                    Name = "Yeti",
                    Email = "yeti@gmail.com",
                    Address = "Xique Xique - BA",
                    Gender = GenderOptions.Male.ToString(),
                    CountryId = Guid.Parse("05C1175A-432E-4AD2-B76A-BE80C8F45A58"),
                    DateOfBirth = DateTime.Parse("2002-05-28"),
                    ReceiveNewsLetters = true,
                },
                new Person()
                {
                    Id = Guid.NewGuid(),
                    Name = "Ronaldo",
                    Email = "ronaldo@gmail.com",
                    Address = "Jussiape",
                    Gender = GenderOptions.Male.ToString(),
                    CountryId = Guid.Parse("76A296FB-F21F-448E-A75D-D66E50FE6777"),
                    DateOfBirth = DateTime.Parse("2002-05-28"),
                    ReceiveNewsLetters = true,
                },
                new Person()
                {
                    Id = Guid.NewGuid(),
                    Name = "Cleiton",
                    Email = "cleitin@gmail.com",
                    Address = "Shibuya",
                    Gender = GenderOptions.Male.ToString(),
                    CountryId = Guid.Parse("82DE0B5E-50F6-4F3D-B43F-16AD763914C1"),
                    DateOfBirth = DateTime.Parse("2002-05-28"),
                    ReceiveNewsLetters = true,
                },
                new Person()
                {
                    Id = Guid.NewGuid(),
                    Name = "Maria",
                    Email = "maria@gmail.com",
                    Address = "São Paulo",
                    Gender = GenderOptions.Female.ToString(),
                    CountryId = Guid.Parse("2B24ADC9-F407-4860-96F9-02C992FF161F"),
                    DateOfBirth = DateTime.Parse("2002-05-28"),
                    ReceiveNewsLetters = true,
                },
                new Person()
                {
                    Id = Guid.NewGuid(),
                    Name = "Cláudio",
                    Email = "craudio@gmail.com",
                    Address = "Belém",
                    Gender = GenderOptions.Male.ToString(),
                    CountryId = Guid.Parse("FF7D0894-44C9-41DF-93AA-4DE74C38BEAA"),
                    DateOfBirth = DateTime.Parse("2002-05-28"),
                    ReceiveNewsLetters = true,
                },
            });
        }
    }

    private PersonResponse ConvertToPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.Country = _countriesService.GetCountryByCountryId(person.CountryId)?.Name;

        return personResponse;
    }

    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest);
        ValidationHelper.ModelValidation(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.Id = Guid.NewGuid();

        _persons.Add(person);

        return ConvertToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
        return _persons.Select(person => person.ToPersonResponse()).ToList();
    }

    public PersonResponse? GetPersonByPersonId(Guid? id)
    {
        return _persons.FirstOrDefault(person => person.Id == id)?.ToPersonResponse();
    }

    public List<PersonResponse> GetFilteredPerson(string searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = GetAllPersons();
        List<PersonResponse> matchingPersons = allPersons;

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) return matchingPersons;

        switch (searchBy)
        {
            case nameof(PersonResponse.Name):
                matchingPersons = allPersons.Where(person =>
                    string.IsNullOrEmpty(person.Name) ||
                    person.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(PersonResponse.Email):
                matchingPersons = allPersons.Where(person =>
                    string.IsNullOrEmpty(person.Email) ||
                    person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(PersonResponse.DateOfBirth):
                matchingPersons = allPersons.Where(person =>
                    person.DateOfBirth == null || person.DateOfBirth.Value.ToString("dd MMMM yyyy")
                        .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(PersonResponse.Gender):
                matchingPersons = allPersons.Where(person =>
                    string.IsNullOrEmpty(person.Gender)
                    || person.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(PersonResponse.CountryId):
                matchingPersons = allPersons.Where(person =>
                    string.IsNullOrEmpty(person.Country)
                    || person.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(PersonResponse.Address):
                matchingPersons = allPersons.Where(person =>
                    string.IsNullOrEmpty(person.Address)
                    || person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            default:
                matchingPersons = allPersons;
                break;
        }

        return matchingPersons;
    }

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortedBy,
        SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortedBy)) return allPersons;

        List<PersonResponse> personResponses = (sortedBy, sortOrder) switch
        {
            (nameof(PersonResponse.Name), SortOrderOptions.ASC) => allPersons
                .OrderBy(person => person.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Name), SortOrderOptions.DESC) => allPersons
                .OrderByDescending(person => person.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons
                .OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons
                .OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons
                .OrderBy(person => person.DateOfBirth).ToList(),
            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons
                .OrderByDescending(person => person.DateOfBirth).ToList(),
            (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons
                .OrderBy(person => person.Age).ToList(),
            (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons
                .OrderByDescending(person => person.Age).ToList(),
            (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons
                .OrderBy(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons
                .OrderByDescending(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons
                .OrderBy(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons
                .OrderByDescending(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons
                .OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons
                .OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons
                .OrderBy(person => person.ReceiveNewsLetters).ToList(),
            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons
                .OrderByDescending(person => person.ReceiveNewsLetters).ToList(),
            _ => allPersons
        };

        return personResponses;
    }

    public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        ArgumentNullException.ThrowIfNull(personUpdateRequest);
        ValidationHelper.ModelValidation(personUpdateRequest);

        Person? person = _persons.FirstOrDefault(person => person.Id == personUpdateRequest.Id);

        if (person is null)
        {
            throw new ArgumentException("Given person id doesn't exist.");
        }

        person.Name = personUpdateRequest.Name;
        person.Email = personUpdateRequest.Email;
        person.DateOfBirth = personUpdateRequest.DateOfBirth;
        person.Gender = personUpdateRequest.Gender.ToString();
        person.CountryId = personUpdateRequest.CountryId;
        person.Address = personUpdateRequest.Address;
        person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        return person.ToPersonResponse();
    }

    public bool DeletePerson(Guid? id)
    {
        ArgumentNullException.ThrowIfNull(id);

        Person? person = _persons.FirstOrDefault(person => person.Id == id);

        if (person == null) return false;

        _persons.RemoveAll(temp => temp.Id == id);

        return true;
    }
}