using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly ICountriesRepository _countriesRepository;

    public CountriesService(ICountriesRepository countriesRepository)
    {
        _countriesRepository = countriesRepository;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        ArgumentNullException.ThrowIfNull(countryAddRequest);

        if (countryAddRequest.Name == null)
        {
            throw new ArgumentException(nameof(countryAddRequest.Name));
        }

        if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.Name) is not null)
        {
            throw new ArgumentException("Given country name already exists.");
        }

        Country country = countryAddRequest.ToCountry();
        country.Id = Guid.NewGuid();

        await _countriesRepository.AddCountry(country);

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null) return null;

        Country? countryFromList =
            await _countriesRepository.GetCountryByCountryId(countryId.Value);

        if (countryFromList == null) return null;

        return countryFromList.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
    {
        MemoryStream memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        int countriesInserted = 0;

        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                string? cellValue = worksheet.Cells[row, 1].Value.ToString();

                if (!string.IsNullOrEmpty(cellValue))
                {
                    string countryName = cellValue;

                    if (await _countriesRepository.GetCountryByCountryName(countryName) is null)
                    {
                        CountryAddRequest countryAddRequest = new() { Name = countryName };
                        await _countriesRepository.AddCountry(countryAddRequest.ToCountry());

                        countriesInserted++;
                    }
                }
            }
        }

        return countriesInserted;
    }
}