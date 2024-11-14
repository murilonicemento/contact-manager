using System.Linq.Expressions;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories;

public class PersonsRepository : IPersonsRepository
{
    private readonly ApplicationDbContext _db;

    public PersonsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Person> AddPerson(Person person)
    {
        await _db.Persons.AddAsync(person);
        await _db.SaveChangesAsync();

        return person;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        return await _db.Persons.Include("Country").ToListAsync();
    }

    public async Task<Person?> GetPersonByPersonId(Guid id)
    {
        return await _db.Persons.Include("Country").FirstOrDefaultAsync(person => person.Id == id);
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
        return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
    }

    public async Task<bool> DeletePersonByPersonId(Guid id)
    {
        _db.Persons.RemoveRange(_db.Persons.Where(person => person.Id == id));
        int rowsDeleted = await _db.SaveChangesAsync();

        return rowsDeleted > 0;
    }

    public async Task<Person> UpdatePerson(Person person)
    {
        Person? matchingPerson = await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.Id == person.Id);

        if (matchingPerson is null) return person;

        matchingPerson.Name = person.Name;
        matchingPerson.Email = person.Email;
        matchingPerson.Country = person.Country;
        matchingPerson.Address = person.Address;
        matchingPerson.Gender = person.Gender;
        matchingPerson.TIN = person.TIN;
        matchingPerson.CountryId = person.CountryId;
        matchingPerson.DateOfBirth = person.DateOfBirth;
        matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

        await _db.SaveChangesAsync();

        return matchingPerson;
    }
}