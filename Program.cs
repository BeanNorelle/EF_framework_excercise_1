global using dotnet_rpg.Models;
global using dotnet_rpg.Services.CharacterService;
global using dotnet_rpg.Services.WeaponService;
global using dotnet_rpg.Services.ApparelService;
global using AutoMapper;
global using dotnet_rpg.Dtos.Character;
global using Microsoft.EntityFrameworkCore;
global using dotnet_rpg.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;


// Create a new application builder
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
// This is like adding tools to a toolbox.
builder.Services.AddControllers(); // Adds the ability to handle requests and return responses
builder.Services.AddEndpointsApiExplorer(); // Adds a tool to help explore the API
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = """Standard Authorization header usig the Bearer scheme. Example: "bearer {token}" """,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IWeaponService, WeaponService>();
builder.Services.AddScoped<IApparelService, ApparelService>();
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

app.UseAuthentication();

app.UseAuthorization(); // Check if the user is authorized to access the API

app.MapControllers(); // Map requests to the appropriate controller method

app.Run(); // Start the application
