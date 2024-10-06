using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Blog.Identity.Models
{
    public static class ClaimsPrincipalExtensions
    {
        public static async Task<IList<Claim>> GetCustomClaimsAsync(this UserManager<IdentityUser> userManager, IdentityUser user, string nome, string sobrenome)
        {
            var additionalClaims = new List<Claim>
            {
                new("FullName", $"{nome.Trim()} {sobrenome.Trim()}"),
                new("FirstName", nome),
                new("LastName", sobrenome),
                new("Email", user.Email)
            };

            var claims = new List<Claim>(await userManager.GetClaimsAsync(user));
            claims.AddRange(additionalClaims);
            return claims;
        }

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentException(nameof(principal));

            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value ?? string.Empty;
        }
    }
}
