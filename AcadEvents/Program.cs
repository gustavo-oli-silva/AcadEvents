using AcadEvents.Repositories;
using AcadEvents.Data;
using AcadEvents.Extensions;
using AcadEvents.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using DotNetEnv;
var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddOpenApi();

builder.Services.AddControllers();

// Configuração do HttpClient para API Externa (Crossref)
builder.Services.AddHttpClient<ICrossrefService, CrossrefService>(client =>
{
    client.BaseAddress = new Uri("https://api.crossref.org/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "AcadEvents/1.0 (mailto:contato@acadevents.com)");
});

// Configuração do HttpClient para API ORCID
builder.Services.AddHttpClient<IOrcidClient, OrcidClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Configuração do DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AcadEventsDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// Injeção de dependência dos repositórios
builder.Services.Inject();

var app = builder.Build();

// Aplicar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AcadEventsDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Aplicando migrations do banco de dados...");
        dbContext.Database.Migrate();
        logger.LogInformation("Migrations aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao aplicar migrations. A aplicação continuará, mas pode haver problemas de conexão.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
