using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace Tests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;

    public CountriesServiceTest()
    {
        _countriesService =
            new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
    }

    #region AddCountry

    // When CountryAddRequests is null, it should throw ArgumentNullException
    [Fact]
    public async Task AddCountry_NullCountry()
    {
        CountryAddRequest? request = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () => { await _countriesService.AddCountry(request); });
    }

    // When CountryName is null, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        CountryAddRequest? request = new CountryAddRequest() { Name = null };

        await Assert.ThrowsAsync<ArgumentException>(async () => { await _countriesService.AddCountry(request); });
    }

    // When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsDuplicate()
    {
        CountryAddRequest? request1 = new CountryAddRequest() { Name = "Brazil" };
        CountryAddRequest? request2 = new CountryAddRequest() { Name = "Brazil" };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _countriesService.AddCountry(request1);
            await _countriesService.AddCountry(request2);
        });
    }

    // When you supply proper country name, it should insert (add) the country to existing list of countries
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        CountryAddRequest? request = new CountryAddRequest() { Name = "Japan" };

        CountryResponse response = await _countriesService.AddCountry(request);
        List<CountryResponse> countriesFromGetAllCountries = await _countriesService.GetAllCountries();

        Assert.True(response.Id != Guid.Empty);
        Assert.Contains(response, countriesFromGetAllCountries);
    }

    #endregion

    #region GetAllCountries

    // The list of countries should be empty by default (before adding any countries)
    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

        Assert.Empty(actualCountryResponseList);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        List<CountryAddRequest> countryAddRequests =
        [
            new()
            {
                Name = "Japan"
            },

            new()
            {
                Name = "Brazil"
            }
        ];

        List<CountryResponse> countriesListFromAddCountry = new List<CountryResponse>();

        foreach (CountryAddRequest countryRequest in countryAddRequests)
        {
            countriesListFromAddCountry.Add(await _countriesService.AddCountry(countryRequest));
        }

        List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

        foreach (CountryResponse expectedCountry in countriesListFromAddCountry)
        {
            Assert.Contains(expectedCountry, actualCountryResponseList);
        }
    }

    #endregion

    #region GetCountryByCountryId

    // If we supply null as CountryId, it should return null as CountryResponse
    [Fact]
    public async Task GetCountryByCountryId_NullCountryId()
    {
        Guid? countryId = null;

        CountryResponse? countryResponseFromGetMethod = await _countriesService.GetCountryByCountryId(countryId);

        Assert.Null(countryResponseFromGetMethod);
    }

    // If we supply a valid CountryId, it should return the matching country details as CountryResponse object
    [Fact]
    public async Task GetCountryByCountryId_ValidCountryId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest { Name = "Japan" };
        CountryResponse countryResponseFromAdd = await _countriesService.AddCountry(countryAddRequest);
        CountryResponse? countryResponseFromGet =
            await _countriesService.GetCountryByCountryId(countryResponseFromAdd.Id);

        Assert.Equal(countryResponseFromAdd, countryResponseFromGet);
    }

    #endregion
}