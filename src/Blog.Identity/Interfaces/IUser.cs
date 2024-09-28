using System.Security.Claims;

namespace Blog.Identity.Interfaces
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
