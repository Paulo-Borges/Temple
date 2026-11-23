namespace Temple.Application.Comum.Excecoes;

public sealed class AcessoNegadoException(string message) : Exception(message);