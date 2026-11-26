using Temple.Domain.Entities;

namespace Temple.Application.Lancamentos;

public static class LancamentoMapeamento
{
    public static LancamentoResponse ParaResponse(this Lancamento lancamento)
    {
        return new LancamentoResponse(
            lancamento.Id,
            lancamento.IgrejaId,
            lancamento.Tipo,
            lancamento.Natureza,
            lancamento.MembroId,
            lancamento.Categoria,
            lancamento.Data,
            lancamento.Valor,
            lancamento.FormaPagamento,
            lancamento.Descricao,
            lancamento.RegistradoPorId,
            lancamento.RegistradoPorNome,
            lancamento.RegistradoEm,
            lancamento.Estornado,
            lancamento.MotivoDoEstorno,
            lancamento.EstornadoPorId,
            lancamento.EstornadoPorNome,
            lancamento.EstornadoEm);
    }
}