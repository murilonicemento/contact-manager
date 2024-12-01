using ContactManager.StartupExtensions;
using Serilog;

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

builder.Services.ConfigureService(builder.Configuration);

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