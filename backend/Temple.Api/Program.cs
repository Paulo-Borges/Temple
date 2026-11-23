using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Temple.Api.Autorizacao;
using Temple.Api.Configuracao;
using Temple.Api.Tratamento;
using Temple.Application;
using Temple.Application.Comum.Acesso;
using Temple.Infrastructure;
using Temple.Infrastructure.Identidade;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.ObterObrigatorio("ConnectionStrings:TempleDb");

OpcoesJwt opcoesJwt = new(
    emissor: builder.Configuration.ObterObrigatorio("Jwt:Emissor"),
    audiencia: builder.Configuration.ObterObrigatorio("Jwt:Audiencia"),
    chave: builder.Configuration.ObterObrigatorio("Jwt:Chave"),
    expiracaoEmHoras: builder.Configuration.GetValue<int>("Jwt:ExpiracaoEmHoras"));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString, builder.Environment.IsDevelopment(), opcoesJwt);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioAtual, UsuarioAtual>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = opcoesJwt.Emissor,
            ValidateAudience = true,
            ValidAudience = opcoesJwt.Audiencia,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = opcoesJwt.CriarChaveDeAssinatura(),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = JwtRegisteredClaimNames.Name,
             RoleClaimType = ClaimsDoTemple.Perfil
        };
    });

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build())
        .AdicionarPoliticasDoTemple();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorGlobalDeExcecoes>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.RespectNullableAnnotations = true;
        options.JsonSerializerOptions.RespectRequiredConstructorParameters = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi(options => options.AddDocumentTransformer<SegurancaBearerNoOpenApi>());

var app = builder.Build();
await app.Services.CriarPerfisAsync();

if (app.Environment.IsDevelopment())
{
    await app.Services.CriarPastorInicialAsync(
        nome: app.Configuration.ObterObrigatorio("PastorInicial:Nome"),
        email: app.Configuration.ObterObrigatorio("PastorInicial:Email"),
        senha: app.Configuration.ObterObrigatorio("PastorInicial:Senha"));
}

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Temple API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

