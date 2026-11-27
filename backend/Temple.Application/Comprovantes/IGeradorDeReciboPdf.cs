namespace Temple.Application.Comprovantes;

public interface IGeradorDeReciboPdf
{
    byte[] Gerar(DadosDoRecibo dados);
}