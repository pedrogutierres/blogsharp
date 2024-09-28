using Blog.Identity.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Blog.Identity.Models
{
    public class AspNetUser : IUser
    {
        private readonly IHttpContextAccessor _accessor;

        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string? Nome => _accessor.HttpContext?.User?.Identity?.Name;

        public IEnumerable<Claim> RetornarClaims() => _accessor.HttpContext?.User?.Claims ?? [];

        public Guid? UsuarioId()
        {
            return Autenticado() ? Guid.Parse(_accessor.HttpContext.User.GetUserId()) : null;
        }

        public bool Autenticado()
        {
            return _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public bool Administrador()
        {
            return _accessor.HttpContext?.User?.IsInRole("Admin") ?? false;
        }
    }
}
