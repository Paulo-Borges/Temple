using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Certificados;

namespace Temple.Api.Controllers;

[ApiController]
[Authorize(Policy = Permissoes.EmitirCertificados)]
public sealed class CertificadosController(CertificadoService certificadoService) : ControllerBase
{
    private const string TipoPdf = "application/pdf";

    [HttpPost("api/membros/{membroId:guid}/historico-cargos/{historicoCargoId:guid}/certificado")]
    [ProducesResponseType<Stream>(StatusCodes.Status200OK, TipoPdf)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<FileContentResult> EmitirDePromocao(Guid membroId, Guid historicoCargoId, CancellationToken cancellationToken)
    {
        CertificadoPdf certificado = await certificadoService.EmitirDePromocaoAsync(membroId, historicoCargoId, cancellationToken);

        return File(certificado.Conteudo, TipoPdf, certificado.NomeDoArquivo);
    }

    [HttpGet("api/certificados/{numero:int}")]
    [ProducesResponseType<Stream>(StatusCodes.Status200OK, TipoPdf)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<FileContentResult> ObterSegundaVia(int numero, CancellationToken cancellationToken)
    {
        CertificadoPdf certificado = await certificadoService.ObterSegundaViaAsync(numero, cancellationToken);

        return File(certificado.Conteudo, TipoPdf, certificado.NomeDoArquivo);
    }
}