using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsService> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public PersonsService(IPersonsRepository personsRepository, ILogger<PersonsService> logger,
        IDiagnosticContext diagnosticContext)
    {
        _personsRepository = personsRepository;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest);
        ValidationHelper.ModelValidation(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.Id = Guid.NewGuid();

        await _personsRepository.AddPerson(person);
        // _db.sp_InsertPerson(person);

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        _logger.LogInformation("GetAllPersons of PersonsService");

        List<Person> persons = await _personsRepository.GetAllPersons();

        return persons.Select(person => person.ToPersonResponse()).ToList();

        // return _db.sp_GetAllPersons().Select(person => person.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? id)
    {
        if (id is null) return null;

        Person? person = await _personsRepository.GetPersonByPersonId(id.Value);

        return person?.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? searchString)
    {
        _logger.LogInformation("GetFilteredPerson of PersonService");

        List<PersonResponse> allPersons;

        using (Operation.Time("Time for filter persons from database"))
        {
            allPersons = searchBy switch
            {
                nameof(PersonResponse.Name) =>
                    (await _personsRepository.GetFilteredPersons(person =>
                        person.Name.Contains(searchString)))
                    .Select(temp => temp.ToPersonResponse()).ToList(),
                nameof(PersonResponse.Email) =>
                    (await _personsRepository.GetFilteredPersons(person =>
                        person.Email.Contains(searchString)))
                    .Select(temp => temp.ToPersonResponse()).ToList(),
                nameof(PersonResponse.DateOfBirth) =>
                    (await _personsRepository.GetFilteredPersons(person =>
                        person.DateOfBirth.Value.ToString("dd MMMM yyyy")
                            .Contains(searchString)))
                    .Select(temp => temp.ToPersonResponse()).ToList(),
                nameof(PersonResponse.Gender) =>
                    (await _personsRepository.GetFilteredPersons(person =>
                        person.Gender.Contains(searchString)))
                    .Select(temp => temp.ToPersonResponse()).ToList(),
                nameof(PersonResponse.CountryId) =>
                    (await _personsRepository.GetFilteredPersons(person =>
                        person.Country.Name.Contains(searchString)))
                    .Select(temp => temp.ToPersonResponse()).ToList(),
                nameof(PersonResponse.Address) =>
                    (await _personsRepository.GetFilteredPersons(person =>
                        person.Address.Contains(searchString)))
                    .Select(temp => temp.ToPersonResponse()).ToList(),
                _ => (await _personsRepository.GetAllPersons()).Select(temp => temp.ToPersonResponse()).ToList(),
            };
        }

        _diagnosticContext.Set("Persons", allPersons);

        return allPersons;
    }

    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortedBy,
        SortOrderOptions sortOrder)
    {
        _logger.LogInformation("GetSortedPersons of PersonService");
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

        Person? person =
            await _personsRepository.GetPersonByPersonId(personUpdateRequest.Id);

        if (person is null) throw new ArgumentException("Given person id doesn't exist.");

        await _personsRepository.UpdatePerson(person);

        return person.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? id)
    {
        ArgumentNullException.ThrowIfNull(id);

        Person? person = await _personsRepository.GetPersonByPersonId(id.Value);

        if (person == null) return false;

        await _personsRepository.DeletePersonByPersonId(person.Id);

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
            (await _personsRepository.GetAllPersons()).Select(temp => temp.ToPersonResponse()).ToList();

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

    public async Task<MemoryStream> GetPersonExcel()
    {
        MemoryStream memoryStream = new();

        using (ExcelPackage excelPackage = new())
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");

            worksheet.Cells["A1"].Value = "Person Name";
            worksheet.Cells["B1"].Value = "Email";
            worksheet.Cells["C1"].Value = "Date of Birth";
            worksheet.Cells["D1"].Value = "Age";
            worksheet.Cells["E1"].Value = "Gender";
            worksheet.Cells["F1"].Value = "Country";
            worksheet.Cells["G1"].Value = "Address";
            worksheet.Cells["H1"].Value = "Receive News Letter";

            using (ExcelRange headerCells = worksheet.Cells["A1:h1"])
            {
                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Lime);
                headerCells.Style.Font.Bold = true;
            }

            int row = 2;
            List<PersonResponse> persons =
                (await _personsRepository.GetAllPersons()).Select(temp => temp.ToPersonResponse()).ToList();

            foreach (var person in persons)
            {
                worksheet.Cells[row, 1].Value = person.Name;
                worksheet.Cells[row, 2].Value = person.Email;
                worksheet.Cells[row, 3].Value = person.DateOfBirth?.ToString("yyyy MMMM dd");
                worksheet.Cells[row, 4].Value = person.Age;
                worksheet.Cells[row, 5].Value = person.Gender;
                worksheet.Cells[row, 6].Value = person.Country;
                worksheet.Cells[row, 7].Value = person.Address;
                worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                row++;
            }

            worksheet.Cells[$"A1:H{row}"].AutoFitColumns();
            await excelPackage.SaveAsync();
        }

        memoryStream.Position = 0;

        return memoryStream;
    }
}