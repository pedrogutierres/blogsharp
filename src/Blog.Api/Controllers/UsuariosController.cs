using Blog.Api.Authentication;
using Blog.Api.ViewModels.Usuarios;
using Blog.Business.Exceptions;
using Blog.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="autenticacaoService">Serviço para autenticação do usuário utilizando atualmente Identity</param>
        /// <param name="jwtTokenGenerate">Classe responsável por gerar o token JWT</param>
        public UsuariosController(
            AutenticacaoService autenticacaoService,
            JwtTokenGenerate jwtTokenGenerate)
        {
            _autenticacaoService = autenticacaoService;
            _jwtTokenGenerate = jwtTokenGenerate;
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
        [HttpPost("registrar")]
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
    }
}
