using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
/// Acts as a DTO for inserting a new person
/// </summary>
public class PersonAddRequest
{
    [Required(ErrorMessage = "Person name can't be blank.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Email can't be blank.")]
    [EmailAddress(ErrorMessage = "Email value should be a valid email.")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [DataType(DataType.Date)] public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender can't be blank")]
    public GenderOptions? Gender { get; set; }

    [Required(ErrorMessage = "Please select a country")]
    public Guid? CountryId { get; set; }

    [Required(ErrorMessage = "Address can't be blank")]
    public string? Address { get; set; }

    public bool? ReceiveNewsLetters { get; set; }

    /// <summary>
    /// Converts the current object of PersonAddRequest into a new object of Person type
    /// </summary>
    /// <returns>Person object</returns>
    public Person ToPerson()
    {
        return new Person()
        {
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