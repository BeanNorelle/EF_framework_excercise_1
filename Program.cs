global using dotnet_rpg.Models;
global using dotnet_rpg.Services.CharacterService;
global using AutoMapper;
global using dotnet_rpg.Dtos.Character;

public class Program
{
    private static void Main(string[] args)
    {
        // Create a new application builder
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // This is like adding tools to a toolbox.
        builder.Services.AddControllers(); // Adds the ability to handle requests and return responses
        builder.Services.AddEndpointsApiExplorer(); // Adds a tool to help explore the API
        builder.Services.AddSwaggerGen(); // Adds a tool to generate API documentation
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        builder.Services.AddScoped<ICharacterService, CharacterService>();

        // Build the application
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        // This is like setting up the assembly line for making something.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(); // Use the API documentation tool
            app.UseSwaggerUI(); // Use the API explorer tool
        }

        app.UseHttpsRedirection(); // Redirect all HTTP requests to HTTPS

        app.UseAuthorization(); // Check if the user is authorized to access the API

        app.MapControllers(); // Map requests to the appropriate controller method

        app.Run(); // Start the application
    }
}
