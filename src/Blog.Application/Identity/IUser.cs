using System.Security.Claims;

namespace Blog.Application.Identity
{
    public interface IUser
    {
        string Nome { get; }
        Guid? UsuarioId();
        bool Autenticado();
        bool Administrador();
        IEnumerable<Claim> RetornarClaims();
    }
}
