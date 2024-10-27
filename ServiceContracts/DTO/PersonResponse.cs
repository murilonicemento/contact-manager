using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
/// Represents DTO class that is used as return type of most method os Persons Service
/// </summary>
public class PersonResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public bool? ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }

    /// <summary>
    /// Compares the current object data with the parameter obj
    /// </summary>
    /// <param name="obj">The PersonResponse Object to compare</param>
    /// <returns>True or false, indicating whether all person details are matched with the specified parameter object</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != typeof(PersonResponse)) return false;

        PersonResponse person = (PersonResponse)obj;

        return Id == person.Id && Name == person.Name && Email == person.Email && DateOfBirth == person.DateOfBirth &&
               Gender == person.Gender && CountryId == person.CountryId && Country == person.Country &&
               Address == person.Address && ReceiveNewsLetters == person.ReceiveNewsLetters && Age == person.Age;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return
            $"Person Id: {Id}, Person name: {Name}, Email: {Email}, Date Of Birth: {DateOfBirth}, Gender: {Gender}, CountryId: {CountryId}, Country: {Country}, Address: {Address}, Receive News Letters: {ReceiveNewsLetters}, Age: {Age}";
    }

    public PersonUpdateRequest ToPersonUpdateRequest()
    {
        return new PersonUpdateRequest()
        {
            Id = Id,
            Name = Name,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = (GenderOptions) Enum.Parse(typeof(GenderOptions), Gender, true),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}

public static class PersonExtensions
{
    /// <summary>
    /// An extension method to convert an object of Person into PersonResponse class
    /// </summary>
    /// <param name="person">The person object to convert</param>
    /// <returns>Return the converted PersonResponse object</returns>
    public static PersonResponse ToPersonResponse(this Person person)
    {
        return new PersonResponse()
        {
            Id = person.Id,
            Name = person.Name,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            CountryId = person.CountryId,
            Address = person.Address,
            ReceiveNewsLetters = person.ReceiveNewsLetters,
            Age = (person.DateOfBirth != null)
                ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25)
                : null
        };
    }
}