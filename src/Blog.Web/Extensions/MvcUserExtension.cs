using Blog.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Blog.Web.Extensions
{
    public static class MvcUserExtension
    {
        public static string FirstName(this ClaimsPrincipal user)
        {
            return user.FindFirst("FirstName")?.Value;
        }
        public static string FullName(this ClaimsPrincipal user)
        {
            return user.FindFirst("FullName")?.Value;

        }
        public static string DisplayName(this ClaimsPrincipal user)
        {
            var fullName = user.FullName();
            if (!string.IsNullOrEmpty(fullName)) return fullName;
            var firstName = user.FirstName();
            if (!string.IsNullOrEmpty(firstName)) return firstName;
            return user.Identity?.Name;
        }

        public static async Task<IList<Claim>> GetCustomClaimsAsync(this UserManager<IdentityUser> userManager, IdentityUser user, Autor autor)
        {
            var additionalClaims = new List<Claim>
            {
                new("FullName", $"{autor.Nome.Trim()} {autor.Sobrenome.Trim()}"),
                new("FirstName", autor.Nome),
                new("LastName", autor.Sobrenome),
                new("Email", user.Email),
                new("UserId", user.Id)
            };

            var claims = new List<Claim>(await userManager.GetClaimsAsync(user));
            claims.AddRange(additionalClaims);
            return claims;
        }
    }
}
