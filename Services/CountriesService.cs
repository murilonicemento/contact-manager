using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly PersonsDbContext _db;

    public CountriesService(PersonsDbContext personsDbContext)
    {
        _db = personsDbContext;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (countryAddRequest == null)
        {
            throw new ArgumentNullException(nameof(countryAddRequest));
        }

        if (countryAddRequest.Name == null)
        {
            throw new ArgumentException(nameof(countryAddRequest.Name));
        }

        if (await _db.Countries.CountAsync(country => country.Name == countryAddRequest.Name) > 0)
        {
            throw new ArgumentException("Given country name already exists.");
        }

        Country country = countryAddRequest.ToCountry();

        country.Id = Guid.NewGuid();

        _db.Countries.Add(country);
        await _db.SaveChangesAsync();

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null) return null;

        Country? countryFromList = await _db.Countries.FirstOrDefaultAsync(country => country.Id == countryId);

        if (countryFromList == null) return null;

        return countryFromList.ToCountryResponse();
    }
}