using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Temple.Application.Comum.Excecoes;
using Temple.Domain.Exceptions;

namespace Temple.Api.Tratamento;

internal sealed class TratadorGlobalDeExcecoes(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails? problemDetails = exception switch
        {
            ValidationException validationException => CriarProblemaDeValidacao(validationException),
            DomainException => CriarProblema(StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada.", exception.Message),
            NaoEncontradoException => CriarProblema(StatusCodes.Status404NotFound, "Recurso não encontrado.", exception.Message),
            ConflitoException => CriarProblema(StatusCodes.Status409Conflict, "Conflito com um recurso existente.", exception.Message),
            _ => null
        };

        if (problemDetails is not { Status: int status })
        {
            return false;
        }

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }

    private static ProblemDetails CriarProblema(int status, string titulo, string detalhe)
    {
        return new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe
        };
    }

    private static HttpValidationProblemDetails CriarProblemaDeValidacao(ValidationException validationException)
    {
        Dictionary<string, string[]> erros = validationException.Errors
            .GroupBy(erro => ParaCamelCase(erro.PropertyName))
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.Select(erro => erro.ErrorMessage).ToArray());

        return new HttpValidationProblemDetails(erros)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Um ou mais campos são inválidos."
        };
    }

    private static string ParaCamelCase(string nomeDaPropriedade)
    {
        IEnumerable<string> partes = nomeDaPropriedade
            .Split('.')
            .Select(JsonNamingPolicy.CamelCase.ConvertName);

        return string.Join('.', partes);
    }
}
