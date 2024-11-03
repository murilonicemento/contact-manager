using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public class PersonsDbContext : DbContext
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<Person> Persons { get; set; }

    public PersonsDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");

        // Seed Data
        string countriesJson = File.ReadAllText("countries.json");
        List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

        foreach (Country country in countries)
        {
            modelBuilder.Entity<Country>().HasData(country);
        }

        string personsJson = File.ReadAllText("persons.json");
        List<Person>? persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

        foreach (Person person in persons)
        {
            modelBuilder.Entity<Person>().HasData(person);
        }
    }

    public List<Person> sp_GetAllPersons()
    {
        return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
    }
}