using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Temple.Application.Comprovantes;

namespace Temple.Infrastructure.Comprovantes;

internal sealed class GeradorDeReciboPdf : IGeradorDeReciboPdf
{
    private const string CorPrincipal = "#1F3A5F";

    public byte[] Gerar(DadosDoRecibo dados)
    {
        ArgumentNullException.ThrowIfNull(dados);

        Document documento = Document.Create(container =>
        {
            container.Page(pagina =>
            {
                pagina.Size(PageSizes.A5.Landscape());
                pagina.Margin(1, Unit.Centimetre);
                pagina.PageColor(Colors.White);
                pagina.DefaultTextStyle(estilo => estilo.FontSize(11).FontColor(Colors.Black));

                pagina.Content()
                    .Border(1.5f)
                    .BorderColor(CorPrincipal)
                    .Padding(14)
                    .AlignMiddle()
                    .Column(coluna =>
                    {
                        // Cabeçalho: a igreja à esquerda, o título e o valor à direita.
                        coluna.Item().Row(linha =>
                        {
                            linha.RelativeItem().Column(igreja =>
                            {
                                igreja.Item().Text(dados.NomeDaIgreja.ToUpperInvariant()).FontSize(13).Bold().FontColor(CorPrincipal);
                                igreja.Item().Text($"CNPJ {dados.CnpjDaIgreja}").FontSize(9).FontColor(Colors.Grey.Darken1);
                            });

                            linha.ConstantItem(170).Column(titulo =>
                            {
                                titulo.Item().AlignRight().Text("RECIBO DE DÍZIMO").FontSize(13).Bold().FontColor(CorPrincipal);
                                titulo.Item()
                                    .PaddingTop(4)
                                    .Border(1)
                                    .BorderColor(CorPrincipal)
                                    .PaddingVertical(4)
                                    .AlignCenter()
                                    .Text(dados.Valor)
                                    .FontSize(14)
                                    .Bold();
                            });
                        });

                        coluna.Item().PaddingVertical(10).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);

                        coluna.Item().Text(dados.Texto).FontSize(12).LineHeight(1.4f).Justify();

                        if (dados.Observacao is string observacao)
                        {
                            coluna.Item().PaddingTop(6).Text($"Observação: {observacao}").FontSize(10).Italic();
                        }

                        coluna.Item().PaddingTop(14).AlignRight().Text(dados.LocalEData);

                        coluna.Item()
                            .PaddingTop(32)
                            .AlignCenter()
                            .Width(220)
                            .Column(assinatura =>
                            {
                                assinatura.Item().LineHorizontal(1).LineColor(Colors.Black);
                                assinatura.Item().PaddingTop(3).AlignCenter().Text("Tesouraria").FontSize(10);
                            });

                        coluna.Item()
                            .PaddingTop(12)
                            .Text(dados.Rodape)
                            .FontSize(7)
                            .FontColor(Colors.Grey.Darken1);
                    });
            });
        });

        return documento.GeneratePdf();
    }
}