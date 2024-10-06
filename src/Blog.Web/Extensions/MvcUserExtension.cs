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
    }
}
