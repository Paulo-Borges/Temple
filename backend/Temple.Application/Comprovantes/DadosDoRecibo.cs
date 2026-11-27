namespace Temple.Application.Comprovantes;

// Tudo o que o recibo precisa, já em texto: o gerador só cuida do layout.
public sealed record DadosDoRecibo(
    string NomeDaIgreja,
    string CnpjDaIgreja,
    string Valor,
    string Texto,
    string? Observacao,
    string LocalEData,
    string Rodape);