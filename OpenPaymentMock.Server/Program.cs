using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Server.Endpoints;
using OpenPaymentMock.Server.Persistance;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Configuration.GetConnectionString("Sqlite") is not null)
{
    builder.Services.AddDbContext<ApplicationDbContext>(_ => _.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
}
else
{
    throw new InvalidOperationException("No connection string found.");
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();

app.MapApiEndpoints();

app.MapFallbackToFile("index.html");

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();
}

app.Run();
