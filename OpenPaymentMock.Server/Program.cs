using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Server.Endpoints;
using OpenPaymentMock.Server.Extensions;
using OpenPaymentMock.Server.Middlewares;
using OpenPaymentMock.Server.Options;
using OpenPaymentMock.Server.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AdminOptions>()
    .Bind(builder.Configuration.GetSection(AdminOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

// For minimap api
builder.Services.ConfigureHttpJsonOptions(_ => _.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// For swashbuckle
builder.Services.Configure<JsonOptions>(_ => _.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

if (builder.Configuration.GetConnectionString("Sqlite") is not null)
{
    builder.Services.AddDbContext<ApplicationDbContext>(_ => _.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
}
else
{
    throw new InvalidOperationException("No connection string found.");
}

var app = builder.Build();

app.UseSwaggerConfig();

app.UseStaticFiles();
app.UseRouting();

app.UseApiKeyMiddleware();
app.UsePartnerAccessKeyMiddleware();

app.MapApiEndpoints();

app.MapFallbackToFile("index.html");

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();
}

app.Run();
