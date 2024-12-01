using ContactManager.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();
// builder.Logging.AddDebug();
// builder.Logging.AddEventLog();

builder.Host.UseSerilog(
    (context, services, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom
            .Configuration(context.Configuration) // read configuration settings from built-in IConfiguration
            .ReadFrom.Services(services); // read out current app's services and make them available to serilog
    });

// adds filter globally
builder.Services.AddControllersWithViews(options =>
{
    // options.Filters.Add<ResponseHeaderActionFilter>(5);
    options.Filters.Add(new ResponseHeaderActionFilter("My-Key-Global", "My-value-Global", 2));
});

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);
});

builder.Services.AddTransient<PersonsListActionFilter>();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                            HttpLoggingFields.ResponsePropertiesAndHeaders;
});

var app = builder.Build();

app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();

if (!builder.Environment.IsEnvironment("Test"))
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", "Rotativa");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program
{
} // make the auto-generated program accessible programmatically 