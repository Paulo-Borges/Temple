namespace Temple.Application.Autenticacao;

public interface IGeradorDeToken
{
    LoginResponse Gerar(UsuarioAutenticado usuario);
}
