using Temple.Domain.Entities;

namespace Temple.Application.Apresentacoes;

public static class ApresentacaoMapeamento
{
    public static ApresentacaoResponse ParaResponse(this ApresentacaoCrianca apresentacao)
    {
        return new ApresentacaoResponse(
            apresentacao.Id,
            apresentacao.IgrejaId,
            apresentacao.NomeDaCrianca,
            apresentacao.DataNascimentoDaCrianca,
            apresentacao.NomeDoPai,
            apresentacao.NomeDaMae,
            apresentacao.DataApresentacao,
            apresentacao.Oficiante);
    }
}