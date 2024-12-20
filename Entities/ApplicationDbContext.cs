using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Person> Persons { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
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

        // Fluent API
        modelBuilder.Entity<Person>()
            .Property(temp => temp.TIN)
            .HasColumnName("TaxIdentificationNumber")
            .HasColumnType("varchar(8)")
            .HasDefaultValue("ABCD1234");

        // modelBuilder.Entity<Person>().HasIndex(temp => temp.Id).IsUnique();
        modelBuilder.Entity<Person>()
            .HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8"); // s_Ctring length deve ser 8

        // Table Relations
        // modelBuilder.Entity<Person>(entity =>
        // {
        //     entity.HasOne<Country>(temp => temp.Country).WithMany(p => p.Person).HasForeignKey(p => p.CountryId);
        // });
    }

    public List<Person> sp_GetAllPersons()
    {
        return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
    }

    public int sp_InsertPerson(Person person)
    {
        SqlParameter[] parameters =
        [
            new("@Id", person.Id),
            new("@Name", person.Name),
            new("@Email", person.Email),
            new("@DateOfBirth", person.DateOfBirth),
            new("@Gender", person.Gender),
            new("@CountryId", person.CountryId),
            new("@Address", person.Address),
            new("@ReceiveNewsLetters", person.ReceiveNewsLetters)
        ];

        return Database.ExecuteSqlRaw(
            "EXECUTE [dbo].[InsertPerson] @Id, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters",
            parameters);
    }
}