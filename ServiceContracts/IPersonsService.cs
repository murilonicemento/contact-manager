using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Person entity
/// </summary>
public interface IPersonsService
{
    /// <summary>
    /// Adds a new person into the list of persons
    /// </summary>
    /// <param name="personAddRequest">Person to add</param>
    /// <returns>Returns the same person details, along with newly generated person id</returns>
    PersonResponse AddPerson(PersonAddRequest? personAddRequest);

    /// <summary>
    /// Returns all persons
    /// </summary>
    /// <returns>return a list of PersonResponse object</returns>
    List<PersonResponse> GetAllPersons();

    /// <summary>
    /// Returns the person object based on the given person id
    /// </summary>
    /// <param name="id">Person id to search</param>
    /// <returns>Matching person object</returns>
    PersonResponse? GetPersonByPersonId(Guid? id);

    /// <summary>
    /// Return all person objects that matches with the given search field and search string
    /// </summary>
    /// <param name="searchBy">Search field to search</param>
    /// <param name="searchString">Search string to search</param>
    /// <returns>Returns all matching persons based on the given search field and search string</returns>
    List<PersonResponse> GetFilteredPerson(string searchBy, string? searchString);

    /// <summary>
    /// Returns sorted list of persons
    /// </summary>
    /// <param name="allPersons">Represents list of persons to sort</param>
    /// <param name="sortedBy">Name of the property (key), based on which the persons should be sorted</param>
    /// <param name="sortOrder">ASC or DESC</param>
    /// <returns>Returns sorted persons as PersonResponse list</returns>
    List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortedBy, SortOrderOptions sortOrder);

    /// <summary>
    /// Updates the specified person details based on the given person id
    /// </summary>
    /// <param name="personUpdateRequest"> person details to update</param>
    /// <returns>Returns a PersonResponse object</returns>
    PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    /// <summary>
    /// Deletes a person base on the given id
    /// </summary>
    /// <param name="id">person id to delete</param>
    /// <returns>return true, if the deletion is successful, otherwise false</returns>
    bool DeletePerson(Guid? id);
}