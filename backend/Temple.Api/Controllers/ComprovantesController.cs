using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Comprovantes;

namespace Temple.Api.Controllers;

[ApiController]
[Authorize(Policy = Permissoes.LancarFinanceiro)]
public sealed class ComprovantesController(ComprovanteService comprovanteService) : ControllerBase
{
    private const string TipoPdf = "application/pdf";

    // GET: o recibo é gerado na hora e não grava nada, então pode ser baixado quantas vezes for preciso.
    [HttpGet("api/lancamentos/{lancamentoId:guid}/recibo")]
    [ProducesResponseType<Stream>(StatusCodes.Status200OK, TipoPdf)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<FileContentResult> GerarReciboDeDizimo(Guid lancamentoId, CancellationToken cancellationToken)
    {
        ReciboPdf recibo = await comprovanteService.GerarReciboDeDizimoAsync(lancamentoId, cancellationToken);

        return File(recibo.Conteudo, TipoPdf, recibo.NomeDoArquivo);
    }
}