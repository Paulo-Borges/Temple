using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Temple.Application.Certificados;

namespace Temple.Infrastructure.Certificados;

internal sealed class GeradorDeCertificadoPdf : IGeradorDeCertificadoPdf
{
    private const string CorPrincipal = "#1F3A5F";
    private const string CorDetalhe = "#B8860B";

    public byte[] Gerar(DadosDoCertificado dados)
    {
        ArgumentNullException.ThrowIfNull(dados);

        Document documento = Document.Create(container =>
        {
            container.Page(pagina =>
            {
                pagina.Size(PageSizes.A4.Landscape());
                pagina.Margin(1, Unit.Centimetre);
                pagina.PageColor(Colors.White);
                pagina.DefaultTextStyle(estilo => estilo.FontSize(14).FontColor(Colors.Black));

                pagina.Content()
                    .Border(3)
                    .BorderColor(CorPrincipal)
                    .Padding(6)
                    .Border(1)
                    .BorderColor(CorDetalhe)
                    .PaddingVertical(1.5f, Unit.Centimetre)
                    .PaddingHorizontal(2.5f, Unit.Centimetre)
                    .AlignMiddle()
                    .Column(coluna =>
                    {
                        coluna.Item()
                            .AlignCenter()
                            .Text(dados.NomeDaIgreja.ToUpperInvariant())
                            .FontSize(20)
                            .Bold()
                            .FontColor(CorPrincipal);

                        coluna.Item()
                            .PaddingTop(30)
                            .AlignCenter()
                            .Text(dados.Titulo)
                            .FontSize(32)
                            .Bold()
                            .FontColor(CorDetalhe);

                        coluna.Item()
                            .PaddingTop(40)
                            .Text(dados.Texto)
                            .FontSize(18)
                            .LineHeight(1.5f)
                            .AlignCenter();

                        coluna.Item()
                            .PaddingTop(30)
                            .AlignCenter()
                            .Text(dados.LocalEData)
                            .FontSize(14);

                        coluna.Item()
                            .PaddingTop(70)
                            .Row(linha =>
                            {
                                linha.Spacing(80);
                                linha.RelativeItem().Element(celula => LinhaDeAssinatura(celula, "Pastor"));
                                linha.RelativeItem().Element(celula => LinhaDeAssinatura(celula, "Secretário(a)"));
                            });

                        coluna.Item()
                            .PaddingTop(30)
                            .AlignRight()
                            .Text($"Nº {dados.Numero:D6}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);
                    });
            });
        });

        return documento.GeneratePdf();
    }

    private static void LinhaDeAssinatura(IContainer container, string rotulo)
    {
        container.Column(coluna =>
        {
            coluna.Item().LineHorizontal(1).LineColor(Colors.Black);
            coluna.Item().PaddingTop(4).AlignCenter().Text(rotulo).FontSize(12);
        });
    }
}