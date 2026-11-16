using Temple.Application.Comum;
using Temple.Domain.Entities;

namespace Temple.Application.Igrejas;

public static class IgrejaMapeamento
{
    public static IgrejaResponse ParaResponse(this Igreja igreja)
    {
        return new IgrejaResponse(
            igreja.Id,
            igreja.Nome,
            igreja.Cnpj,
            igreja.Telefone,
            igreja.DataFundacao,
            igreja.Ativa,
            igreja.EhMatriz,
            igreja.IgrejaMatrizId,
            igreja.Endereco.ParaDto());
    }

    public static Igreja ParaMatriz(this CriarIgrejaRequest request)
    {
        return Igreja.CriarMatriz(
            request.Nome,
            request.Cnpj,
            request.Telefone,
            request.DataFundacao,
            request.Ativa,
            request.Endereco.ParaEndereco());
    }

    public static Igreja ParaFilialDe(this CriarIgrejaRequest request, Igreja matriz)
    {
        ArgumentNullException.ThrowIfNull(matriz);

        return matriz.CriarFilial(
            request.Nome,
            request.Cnpj,
            request.Telefone,
            request.DataFundacao,
            request.Ativa,
            request.Endereco.ParaEndereco());
    }
}
