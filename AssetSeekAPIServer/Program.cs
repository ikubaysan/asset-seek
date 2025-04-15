using AssetSeekAPIServer.Data; // For DatabaseInitializer
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


var logDir = Path.Combine(AppContext.BaseDirectory, "Logs");
Directory.CreateDirectory(logDir); // Ensure Logs dir exists

var logPath = Path.Combine(logDir, "log-.log");

builder.Host.UseSerilog((context, services, configuration) =>
{
    var enableFileLogging = context.Configuration.GetValue<bool>("EnableFileLogging", true);

    if (enableFileLogging)
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                shared: true
            );
    }
    else
    {
        configuration
            .MinimumLevel.Information()
            .WriteTo.Console();
    }
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Log a hello message
Console.WriteLine("Using Serilog configuration from appsettings...");
Log.Information("Hello, world!");

// Initialize the database tables before the app starts serving requests.
var connectionString = builder.Configuration.GetConnectionString("AzureSql");
DatabaseInitializer.Initialize(connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
