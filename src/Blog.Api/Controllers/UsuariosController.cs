using Blog.Api.Authentication;
using Blog.Api.ViewModels.Usuarios;
using Blog.Business.Exceptions;
using Blog.Business.Services;
using Blog.Data;
using Blog.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace Blog.Api.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar os usuários
    /// </summary>
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/usuarios")]
    public class UsuariosController : Controller
    {
        private readonly AutenticacaoService _autenticacaoService;
        private readonly JwtTokenGenerate _jwtTokenGenerate;
        private readonly IUser _user;

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="autenticacaoService">Serviço para autenticação do usuário utilizando atualmente Identity</param>
        /// <param name="jwtTokenGenerate">Classe responsável por gerar o token JWT</param>
        public UsuariosController(
            AutenticacaoService autenticacaoService,
            JwtTokenGenerate jwtTokenGenerate,
            IUser user)
        {
            _autenticacaoService = autenticacaoService;
            _jwtTokenGenerate = jwtTokenGenerate;
            _user = user;
        }

        /// <summary>
        /// Realizar o login do usuário
        /// </summary>
        /// <param name="login">Dados do usuário</param>
        /// <returns>Retorna um objeto com o token e outros dados do usuário</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            if (await _autenticacaoService.ValidarLoginUsuarioAsync(login.Email, login.Senha))
                return Ok(await _jwtTokenGenerate.GerarToken(login.Email));

            return BadRequest(new ProblemDetails
            {
                Title = "Falha na autenticação",
                Detail = "E-mail e/ou senha inválidos."
            });
        }

        /// <summary>
        /// Registrar um usuário (autor)
        /// </summary>
        /// <param name="registrar">Dados do usuário (autor)</param>
        /// <returns>Retorna um objeto com o token e outros dados do usuário</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioViewModel registrar)
        {
            try
            {
                var user = await _autenticacaoService.RegistrarUsuarioAsync(registrar.Email, registrar.Senha, registrar.Nome, registrar.Sobrenome);
                if (user != null)
                    return Ok(await _jwtTokenGenerate.GerarToken(user.Email));
            }
            catch (BusinessException ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Falha no registro",
                    Detail = ex.Message,
                };

                var errors = new Dictionary<string, string[]>();

                foreach (var error in ex.Data as Dictionary<string, string> ?? [])
                    errors.Add(error.Key, [error.Value]);

                problemDetails.Extensions.Add("errors", errors);

                return BadRequest(problemDetails);
            }
            catch
            {
                throw;
            }

            return BadRequest(new ProblemDetails
            {
                Title = "Falha no registro",
                Detail = "Não foi possível registrar o usuário."
            });
        }

        /// <summary>
        /// Alterar dados do usuário (autor)
        /// </summary>
        /// <param name="id">ID do usuário (autor)</param>
        /// <param name="alterar">Dados do usuário (autor)</param>
        /// <returns>Retorna OK quando alterar os dados com sucesso</returns>
        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Alterar([FromRoute] Guid id, [FromBody] AlterarUsuarioViewModel alterar, [FromServices] UserManager<IdentityUser> _userManager, [FromServices] ApplicationDbContext _context)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            if (user.Id != _user.UsuarioId().Value.ToString())
                throw new UnauthorizedAccessException("Usuário não autorizado a alterar o cadastro que não pertence a ele.");

            var telefone = await _userManager.GetPhoneNumberAsync(user);
            if (alterar.Telefone != telefone)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, alterar.Telefone);
                if (!result.Succeeded)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Falha no alteração",
                        Detail = "Não foi possível alterar o telefone."
                    });
                }
            }

            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Falha no alteração",
                    Detail = "Autor vinculado ao usuário não foi encontrado."
                });
            }

            autor.Nome = alterar.Nome?.Trim();
            autor.Sobrenome = alterar.Sobrenome?.Trim();

            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Alterar e-mail do usuário (autor)
        /// </summary>
        /// <param name="id">ID do usuário (autor)</param>
        /// <param name="alterar">Dados do e-mail do usuário (autor)</param>
        /// <returns>Retorna OK quando alterar o e-mail com sucesso</returns>
        [HttpPut("{id:guid}/email")]
        [Authorize]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlterarEmail([FromRoute] Guid id, [FromBody] AlterarEmailDoUsuarioViewModel alterar, [FromServices] UserManager<IdentityUser> _userManager)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            if (user.Id != _user.UsuarioId().Value.ToString())
                throw new UnauthorizedAccessException("Usuário não autorizado a alterar o e-mail que não pertence a ele.");

            var email = await _userManager.GetEmailAsync(user);
            if (alterar.NovoEmail != email)
            {
                await _userManager.SetUserNameAsync(user, alterar.NovoEmail);
                await _userManager.UpdateNormalizedUserNameAsync(user);
                var result = await _userManager.ChangeEmailAsync(user, alterar.NovoEmail, await _userManager.GenerateChangeEmailTokenAsync(user, alterar.NovoEmail));
                if (!result.Succeeded)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Falha no alteração",
                        Detail = "Não foi possível alterar o e-mail."
                    });
                }
            }

            return Ok();
        }

        /// <summary>
        /// Alterar senha do usuário (autor)
        /// </summary>
        /// <param name="id">ID do usuário (autor)</param>
        /// <param name="alterar">Dados da senha do usuário (autor)</param>
        /// <returns>Retorna OK quando alterar a senha com sucesso</returns>
        [HttpPut("{id:guid}/senha")]
        [Authorize]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlterarSenha([FromRoute] Guid id, [FromBody] AlterarSenhaDoUsuarioViewModel alterar, [FromServices] UserManager<IdentityUser> _userManager)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            if (user.Id != _user.UsuarioId().Value.ToString())
                throw new UnauthorizedAccessException("Usuário não autorizado a alterar o e-mail que não pertence a ele.");

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, alterar.SenhaAtual, alterar.NovaSenha);
            if (!changePasswordResult.Succeeded)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Falha no registro",
                    Detail = "Não foi possível alterar a senha do usuário",
                };

                var errors = new Dictionary<string, string[]>();

                foreach (var error in changePasswordResult.Errors)
                    errors.Add(error.Code, [error.Description]);

                problemDetails.Extensions.Add("errors", errors);

                return BadRequest(problemDetails);
            }

            return Ok();
        }

        /// <summary>
        /// Deletar dados do usuário (autor)
        /// </summary>
        /// <param name="id">ID do usuário (autor)</param>
        /// <param name="alterar">Dados do usuário (autor)</param>
        /// <returns>Retorna OK quando alterar os dados com sucesso</returns>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(AuthToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deletar([FromRoute] Guid id, [FromBody] DeletarUsuarioViewModel deletar, [FromServices] UserManager<IdentityUser> _userManager)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            if (user.Id != _user.UsuarioId().Value.ToString())
                throw new UnauthorizedAccessException("Usuário não autorizado a excluir cadastro que não pertence a ele.");

            if (!await _userManager.CheckPasswordAsync(user, deletar.Senha))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Falha no alteração",
                    Detail = "Senha incorreta."
                });
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Não foi possível deletar o usuário.");

            await _autenticacaoService.InativarUsuarioAsync(Guid.Parse(user.Id));

            return Ok();
        }
    }
}
