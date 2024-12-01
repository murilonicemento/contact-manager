using ContactManager.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace ContactManager.StartupExtensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureService(this IServiceCollection service, IConfiguration configuration)
    {
        // adds filter globally
        service.AddControllersWithViews(options =>
        {
            var logger = service.BuildServiceProvider()
                .GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

            // options.Filters.Add<ResponseHeaderActionFilter>(5);
            options.Filters.Add(new ResponseHeaderActionFilter(logger)
            {
                Key = "My-Key-Global",
                Value = "My-value-Global",
                Order = 2
            });
        });

        service.AddScoped<ICountriesService, CountriesService>();
        service.AddScoped<IPersonsService, PersonsService>();
        service.AddScoped<ICountriesRepository, CountriesRepository>();
        service.AddScoped<IPersonsRepository, PersonsRepository>();

        service.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ?? string.Empty);
        });

        service.AddTransient<PersonsListActionFilter>();

        service.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                                    HttpLoggingFields.ResponsePropertiesAndHeaders;
        });

        return service;
    }
}