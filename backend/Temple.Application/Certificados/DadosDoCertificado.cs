namespace Temple.Application.Certificados;

// Tudo o que o PDF precisa, já em texto: o gerador só cuida do layout.
public sealed record DadosDoCertificado(
    int Numero,
    string NomeDaIgreja,
    string Titulo,
    string Texto,
    string LocalEData);