using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly List<Country> _countries;

    public CountriesService(bool initialize = true)
    {
        _countries = new List<Country>();

        if (initialize)
        {
            _countries.AddRange(new List<Country>()
            {
                new Country() { Id = Guid.Parse("05C1175A-432E-4AD2-B76A-BE80C8F45A58"), Name = "Japan" },
                new Country() { Id = Guid.Parse("76A296FB-F21F-448E-A75D-D66E50FE6777"), Name = "Brazil" },
                new Country() { Id = Guid.Parse("82DE0B5E-50F6-4F3D-B43F-16AD763914C1"), Name = "USA" },
                new Country() { Id = Guid.Parse("2B24ADC9-F407-4860-96F9-02C992FF161F"), Name = "Argentina" },
                new Country() { Id = Guid.Parse("FF7D0894-44C9-41DF-93AA-4DE74C38BEAA"), Name = "Chile" }
            });
        }
    }

    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (countryAddRequest == null)
        {
            throw new ArgumentNullException(nameof(countryAddRequest));
        }

        if (countryAddRequest.Name == null)
        {
            throw new ArgumentException(nameof(countryAddRequest.Name));
        }

        if (_countries.Where(country => country.Name == countryAddRequest.Name).Count() > 0)
        {
            throw new ArgumentException("Given country name already exists.");
        }

        Country country = countryAddRequest.ToCountry();

        country.Id = Guid.NewGuid();

        _countries.Add(country);

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        return _countries.Select(country => country.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null) return null;

        Country? countryFromList = _countries.FirstOrDefault(country => country.Id == countryId);

        if (countryFromList == null) return null;

        return countryFromList.ToCountryResponse();
    }
}