namespace Temple.Application.Certificados;

public interface IGeradorDeCertificadoPdf
{
    byte[] Gerar(DadosDoCertificado dados);
}