using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Temple.Api.Configuracao;

internal sealed class SegurancaBearerNoOpenApi : IOpenApiDocumentTransformer
{
    private const string NomeDoEsquema = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        OpenApiSecurityScheme esquema = new()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Cole aqui o token recebido em POST /api/autenticacao/login."
        };

        OpenApiComponents componentes = document.Components is OpenApiComponents existentes
            ? existentes
            : new OpenApiComponents();

        Dictionary<string, IOpenApiSecurityScheme> esquemas = new()
        {
            [NomeDoEsquema] = esquema
        };

        componentes.SecuritySchemes = esquemas;
        document.Components = componentes;

        OpenApiSecurityRequirement requisito = new()
        {
            [new OpenApiSecuritySchemeReference(NomeDoEsquema, document)] = []
        };

        document.Security = [requisito];

        return Task.CompletedTask;
    }
}
