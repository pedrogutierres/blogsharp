using Blog.Application.Identity;
using Blog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blog.Api.Authentication
{
    public class JwtTokenGenerate
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _context;

        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        public JwtTokenGenerate(
            ApplicationDbContext context,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        public async Task<AuthToken> GerarToken(string email)
        {
            var identityClaims = new ClaimsIdentity();

            var user = await _userManager.FindByEmailAsync(email) ?? throw new NullReferenceException($"Usuário não encontrado pelo email {email}");
            var roles = await _userManager.GetRolesAsync(user);
            var autor = await _context.Autores.FindAsync(Guid.Parse(user.Id)) ?? throw new NullReferenceException($"Cadastro do autor do usuário não encontrado pelo email {email}");

            identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.UniqueName, user.Email));
            identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            identityClaims.AddClaims(await _userManager.GetCustomClaimsAsync(user, autor.Nome, autor.Sobrenome));

            // Adicionar roles como claims
            foreach (var role in roles ?? [])
            {
                identityClaims.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var dataExpiracao = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Segredo);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = identityClaims,
                Issuer = _jwtSettings.Emissor,
                Audience = _jwtSettings.Audiencia,
                Expires = dataExpiracao,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);

            return new AuthToken
            {
                Result = new AuthResult
                {
                    Access_token = encodedToken,
                    Expires_in = dataExpiracao,
                    User = new AuthUser
                    {
                        Id = Guid.Parse(user.Id),
                        NomeCompleto = $"{autor.Nome.Trim()} {autor.Sobrenome.Trim()}",
                        Email = email,
                        Claims = identityClaims.Claims.Select(c => new AuthClaim { Type = c.Type, Value = c.Value })
                    }
                }
            };
        }
    }
}
