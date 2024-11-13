using Entities;

namespace RepositoryContracts;

/// <summary>
/// Represents data access logic for managing Person Entity
/// </summary>
public interface ICountriesRepository
{
    /// <summary>
    /// Adds a new country object to the data
    /// </summary>
    /// <param name="country">Country object to add</param>
    /// <returns>Return the country object after adding it to the data store</returns>
    Task<Country> AddCountry(Country country);

    /// <summary>
    ///  Returns all countries in the data store
    /// </summary>
    /// <returns>All countries from the table</returns>
    Task<List<Country>> GetAllCountries();

    /// <summary>
    /// Returns a country object based on the given country id otherwise, it returns null
    /// </summary>
    /// <param name="id">Country id to search</param>
    /// <returns>Matching country or null</returns>
    Task<Country?> GetCountryByCountryId(Guid id);

    /// <summary>
    /// Returns a country object based on the given country name otherwise, it returns null
    /// </summary>
    /// <param name="name">Country name to search</param>
    /// <returns>Matching country or null</returns>
    Task<Country?> GetCountryByCountryName(string name);
}