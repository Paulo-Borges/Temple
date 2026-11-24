namespace Temple.Application.Certificados;

public sealed record CertificadoPdf(int Numero, string NomeDoArquivo, byte[] Conteudo);