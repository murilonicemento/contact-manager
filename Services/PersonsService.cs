using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    private readonly PersonsDbContext _db;
    private readonly ICountriesService _countriesService;

    public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
    {
        _db = personsDbContext;
        _countriesService = countriesService;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest);
        ValidationHelper.ModelValidation(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.Id = Guid.NewGuid();

        _db.Persons.Add(person);
        // _db.sp_InsertPerson(person);
        await _db.SaveChangesAsync();

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        List<Person> persons = await _db.Persons.Include("Country").ToListAsync();

        return persons.Select(person => person.ToPersonResponse()).ToList();

        // return _db.sp_GetAllPersons().Select(person => person.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? id)
    {
        Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(person => person.Id == id);

        return person?.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = await GetAllPersons();
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

    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortedBy,
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

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        ArgumentNullException.ThrowIfNull(personUpdateRequest);
        ValidationHelper.ModelValidation(personUpdateRequest);

        Person? person = await _db.Persons.FirstOrDefaultAsync(person => person.Id == personUpdateRequest.Id);

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

        await _db.SaveChangesAsync();

        return person.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? id)
    {
        ArgumentNullException.ThrowIfNull(id);

        Person? person = await _db.Persons.FirstOrDefaultAsync(person => person.Id == id);

        if (person == null) return false;

        _db.Persons.Remove(_db.Persons.First(temp => temp.Id == person.Id));
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<MemoryStream> GetPersonsCSV()
    {
        MemoryStream memoryStream = new();
        StreamWriter streamWriter = new(memoryStream);
        CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture);
        CsvWriter csvWriter = new(streamWriter, csvConfiguration);

        csvWriter.WriteField(nameof(PersonResponse.Name));
        csvWriter.WriteField(nameof(PersonResponse.Email));
        csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
        csvWriter.WriteField(nameof(PersonResponse.Age));
        csvWriter.WriteField(nameof(PersonResponse.Gender));
        csvWriter.WriteField(nameof(PersonResponse.Country));
        csvWriter.WriteField(nameof(PersonResponse.Address));
        csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));

        await csvWriter.NextRecordAsync();

        List<PersonResponse> persons =
            await _db.Persons.Include("Country").Select(person => person.ToPersonResponse()).ToListAsync();

        foreach (PersonResponse person in persons)
        {
            csvWriter.WriteField(person.Name);
            csvWriter.WriteField(person.Email);
            csvWriter.WriteField(person.DateOfBirth?.ToString("yyyy-MM-dd"));
            csvWriter.WriteField(person.Age);
            csvWriter.WriteField(person.Country);
            csvWriter.WriteField(person.Address);
            csvWriter.WriteField(person.ReceiveNewsLetters);

            await csvWriter.NextRecordAsync();
            await csvWriter.FlushAsync();
        }

        await csvWriter.WriteRecordsAsync(persons);

        memoryStream.Position = 0;

        return memoryStream;
    }
}