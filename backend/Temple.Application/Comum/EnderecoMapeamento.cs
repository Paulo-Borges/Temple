using Temple.Domain.ValueObjects;

namespace Temple.Application.Comum;

public static class EnderecoMapeamento
{
    public static EnderecoDto ParaDto(this Endereco endereco)
    {
        return new EnderecoDto(
            endereco.Logradouro,
            endereco.Numero,
            endereco.Complemento,
            endereco.Bairro,
            endereco.Cidade,
            endereco.Uf,
            endereco.Cep);
    }

    public static Endereco ParaEndereco(this EnderecoDto dto)
    {
        return new Endereco(
            dto.Logradouro,
            dto.Numero,
            dto.Complemento,
            dto.Bairro,
            dto.Cidade,
            dto.Uf,
            dto.Cep);
    }
}
