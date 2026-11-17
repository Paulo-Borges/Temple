using System.Text.Json.Serialization;
using Temple.Api.Tratamento;
using Temple.Application;
using Temple.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("TempleDb");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("A connection string 'TempleDb' não foi configurada.");
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString, builder.Environment.IsDevelopment());


builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorGlobalDeExcecoes>();

// Add services to the container.

builder.Services.AddControllers()
 .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.RespectNullableAnnotations = true;
        options.JsonSerializerOptions.RespectRequiredConstructorParameters = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
     app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Temple API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
