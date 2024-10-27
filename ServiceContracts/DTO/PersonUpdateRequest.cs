using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
/// Acts as a DTO for updating a person
/// </summary>
public class PersonUpdateRequest
{
    [Required(ErrorMessage = "Person id is required;")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Person name can't be blank.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Email can't be blank.")]
    [EmailAddress(ErrorMessage = "Email value should be a valid email.")]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool? ReceiveNewsLetters { get; set; }

    /// <summary>
    /// Converts the current object of PersonUpdateRequest into a new object of Person type
    /// </summary>
    /// <returns>Person Object</returns>
    public Person ToPerson()
    {
        return new Person()
        {
            Id = Id,
            Name = Name,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}