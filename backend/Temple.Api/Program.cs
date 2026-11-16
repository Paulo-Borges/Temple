using Microsoft.EntityFrameworkCore;
using Temple.Infrastructure.Persistence;
using Temple.Infrastructure.Persistence.Seed;


var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("TempleDb");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("A connection string 'TempleDb' não foi configurada.");
}

builder.Services.AddDbContext<TempleDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.UseSeeding(DadosDeDesenvolvimento.Semear);
        options.UseAsyncSeeding(DadosDeDesenvolvimento.SemearAsync);
    }
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
