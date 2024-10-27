using System.Data.Common;
using Entities;

namespace ServiceContracts.DTO;

/// <summary>
/// DTO class that is used as return type for most of CountriesService methods
/// </summary>
public class CountryResponse
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != typeof(CountryResponse)) return false;

        CountryResponse countryResponse = (CountryResponse)obj;

        return Id == countryResponse.Id && Name == countryResponse.Name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse()
        {
            Id = country.Id,
            Name = country.Name
        };
    }
}