namespace Temple.Application.Comum.Excecoes;

public sealed class NaoAutorizadoException(string message) : Exception(message);
