using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Repository;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar MongoClient e Database
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient("mongodb://localhost:27017"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase("CadastroAcoesDb"));

// Configs do appsettings (JWT will still be read from appsettings)
var jwtSection = builder.Configuration.GetSection("Jwt");

// Registrar repositório de usuario (implemente abaixo)
builder.Services.AddScoped<IUserRepository, UserRepository>();
// Registrar repositório de produtos
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// Registrar repositórios de clientes e vendors
builder.Services.AddScoped<IUserClientRepository, UserClientRepository>();
builder.Services.AddScoped<IUserVendorRepository, UserVendorRepository>();

// Configurar JWT
// Ler a chave de JWT e garantir que não é nula. Em desenvolvimento, é melhor falhar com mensagem clara
var jwtKeyString = jwtSection.GetValue<string>("Key");
if (string.IsNullOrEmpty(jwtKeyString))
{
    // Falha cedo e explicativa para evitar passar null para Encoding.GetBytes
    throw new InvalidOperationException("JWT 'Key' is not configured. Please set 'Jwt:Key' in appsettings.json or environment variables for the application to start.");
}
var key = Encoding.UTF8.GetBytes(jwtKeyString);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
        ValidateAudience = true,
        ValidAudience = jwtSection.GetValue<string>("Audience"),
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true
    };
});

// Habilitar controllers e static files
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serve um arquivo padrão quando a raiz for acessada. Inclui o index dentro de /layouts.
var defaultFilesOptions = new DefaultFilesOptions();
// busca por "layouts/index.html" primeiro, depois por "index.html" na raiz
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("layouts/index.html");
defaultFilesOptions.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(defaultFilesOptions);
app.UseStaticFiles(); // serve wwwroot
app.UseRouting();

// Global error handling middleware - returns structured error responses
app.UseMiddleware<Middleware.ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
