using NuGet.Frameworks;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace Tests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;

    public CountriesServiceTest()
    {
        _countriesService = new CountriesService(false);
    }

    #region AddCountry

    // When CountryAddRequests is null, it should throw ArgumentNullException
    [Fact]
    public void AddCountry_NullCountry()
    {
        CountryAddRequest? request = null;

        Assert.Throws<ArgumentNullException>(() => { _countriesService.AddCountry(request); });
    }

    // When CountryName is null, it should throw ArgumentException
    [Fact]
    public void AddCountry_CountryNameIsNull()
    {
        CountryAddRequest? request = new CountryAddRequest() { Name = null };

        Assert.Throws<ArgumentException>(() => { _countriesService.AddCountry(request); });
    }

    // When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public void AddCountry_CountryNameIsDuplicate()
    {
        CountryAddRequest? request1 = new CountryAddRequest() { Name = "Brazil" };
        CountryAddRequest? request2 = new CountryAddRequest() { Name = "Brazil" };

        Assert.Throws<ArgumentException>(() =>
        {
            _countriesService.AddCountry(request1);
            _countriesService.AddCountry(request2);
        });
    }

    // When you supply proper country name, it should insert (add) the country to existing list of countries
    [Fact]
    public void AddCountry_ProperCountryDetails()
    {
        CountryAddRequest? request = new CountryAddRequest() { Name = "Japan" };

        CountryResponse response = _countriesService.AddCountry(request);
        List<CountryResponse> countriesFromGetAllCountries = _countriesService.GetAllCountries();

        Assert.True(response.Id != Guid.Empty);
        Assert.Contains(response, countriesFromGetAllCountries);
    }

    #endregion

    #region GetAllCountries

    // The list of countries should be empty by default (before adding any countries)
    [Fact]
    public void GetAllCountries_EmptyList()
    {
        List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

        Assert.Empty(actualCountryResponseList);
    }

    [Fact]
    public void GetAllCountries_AddFewCountries()
    {
        List<CountryAddRequest> countryAddRequests =
            new List<CountryAddRequest>()
            {
                new CountryAddRequest()
                {
                    Name = "Japan"
                },
                new CountryAddRequest()
                {
                    Name = "Brazil"
                },
            };

        List<CountryResponse> countriesListFromAddCountry = new List<CountryResponse>();

        foreach (CountryAddRequest countryRequest in countryAddRequests)
        {
            countriesListFromAddCountry.Add(_countriesService.AddCountry(countryRequest));
        }

        List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

        foreach (CountryResponse expectedCountry in countriesListFromAddCountry)
        {
            Assert.Contains(expectedCountry, actualCountryResponseList);
        }
    }

    #endregion

    #region GetCountryByCountryId

    // If we supply null as CountryId, it should return null as CountryResponse
    [Fact]
    public void GetCountryByCountryId_NullCountryId()
    {
        Guid? countryId = null;

        CountryResponse? countryResponseFromGetMethod = _countriesService.GetCountryByCountryId(countryId);

        Assert.Null(countryResponseFromGetMethod);
    }

    // If we supply a valid CountryId, it should return the matching country details as CountryResponse object
    [Fact]
    public void GetCountryByCountryId_ValidCountryId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest() { Name = "Japan" };
        CountryResponse countryResponseFromAdd = _countriesService.AddCountry(countryAddRequest);
        CountryResponse? countryResponseFromGet = _countriesService.GetCountryByCountryId(countryResponseFromAdd.Id);
        
        Assert.Equal(countryResponseFromAdd, countryResponseFromGet);
    }

    #endregion
}