using System.Globalization;
using Temple.Domain.Enums;

namespace Temple.Application.Comprovantes;

internal static class TextosDoComprovante
{
    private static readonly CultureInfo PortuguesDoBrasil = new("pt-BR");

    // Completa a frase "…referente ao dízimo, pago ___."
    public static string FormaDePagamento(FormaPagamento formaPagamento)
    {
        return formaPagamento switch
        {
            FormaPagamento.Dinheiro => "em dinheiro",
            FormaPagamento.Pix => "por Pix",
            FormaPagamento.Cartao => "no cartão",
            FormaPagamento.Transferencia => "por transferência bancária",
            _ => throw new ArgumentOutOfRangeException(nameof(formaPagamento), formaPagamento, "Forma de pagamento sem texto definido.")
        };
    }

    public static string ValorEmReais(decimal valor)
    {
        return valor.ToString("C", PortuguesDoBrasil);
    }

    // O CNPJ é guardado sem máscara; no papel, ele aparece como 12.345.678/0001-95.
    public static string CnpjFormatado(string cnpj)
    {
        return $"{cnpj[..2]}.{cnpj[2..5]}.{cnpj[5..8]}/{cnpj[8..12]}-{cnpj[12..]}";
    }

    public static string DataEHora(DateTimeOffset momento)
    {
        return momento.ToString("dd/MM/yyyy 'às' HH:mm", PortuguesDoBrasil);
    }

    public static string NomeDoArquivo(DateOnly data, Guid lancamentoId)
    {
        return $"recibo-dizimo-{data:yyyy-MM-dd}-{lancamentoId:N}.pdf";
    }
}