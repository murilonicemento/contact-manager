using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Country entity
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// Adds a country object to the list of countries
    /// </summary>
    /// <param name="countryAddRequest">Country object to add</param>
    /// <returns>Return the country object after adding it (including newly generated country id)</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// Return all countries from the list
    /// </summary>
    /// <returns>All countries from the list as List of CountryResponse></returns>
    Task<List<CountryResponse>> GetAllCountries();

    /// <summary>
    /// Returns a country object based on the given id
    /// </summary>
    /// <param name="countryId">CountryId (guid) to search</param>
    /// <returns>Matching country as CountryResponse object</returns>
    Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);
}