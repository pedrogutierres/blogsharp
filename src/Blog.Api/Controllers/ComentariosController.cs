using AutoMapper;
using Blog.Api.ViewModels;
using Blog.Api.ViewModels.Comentarios;
using Blog.Application.Services;
using Blog.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Blog.Api.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar os comentários
    /// </summary>
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/comentarios")]
    public class ComentariosController : Controller
    {
        private readonly ComentariosService _comentariosService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="comentarioService">Serviço para gerenciamento dos comentários com a base de dados</param>
        /// <param name="mapper">Interface para facilitar a transferência de dados entre Models e DTO's</param>
        public ComentariosController(ComentariosService comentarioService, IMapper mapper)
        {
            _comentariosService = comentarioService;
            _mapper = mapper;
        }

        /// <summary>
        /// Obter todos os comentários de um Post
        /// </summary>
        /// <param name="postId">ID do post</param>
        /// <returns>Retorna uma lista dos comentários</returns>
        [HttpGet("posts/{postId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ComentarioViewModel>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<ComentarioViewModel>> ObterComentarios([FromRoute] Guid postId)
        {
            return _mapper.Map<IEnumerable<Comentario>, IEnumerable<ComentarioViewModel>>(await _comentariosService.ObterComentariosAsync(postId));
        }

        /// <summary>
        /// Publicar um comentário em um Post
        /// </summary>
        /// <param name="postId">ID do post</param>
        /// <param name="viewModel">Dados do comentário</param>
        /// <returns>Retorna o ID do comentário publicado</returns>
        [HttpPost("posts/{postId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PublicarComentario([FromRoute] Guid postId, [FromBody] PublicarComentarioViewModel viewModel)
        {
            var comentario = _mapper.Map<PublicarComentarioViewModel, Comentario>(viewModel) ?? throw new ArgumentNullException();
            comentario.PostId = postId;

            await _comentariosService.PublicarComentarioAsync(comentario);

            return Ok(new ResponseSuccess(comentario.Id));
        }

        /// <summary>
        /// Editar um comentário
        /// </summary>
        /// <param name="id">ID do comentário</param>
        /// <param name="viewModel">Dados do comentário alterado</param>
        /// <returns>Retorna o ID do comentário alterado</returns>
        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditarComentario([FromRoute] Guid id, [FromBody] EditarComentarioViewModel viewModel)
        {
            var comentario = _mapper.Map<EditarComentarioViewModel, Comentario>(viewModel) ?? throw new ArgumentNullException();
            comentario.Id = id;

            var comentarioAlterado = await _comentariosService.EditarComentarioAsync(comentario);
            if (comentarioAlterado == null)
                return NotFound();

            return Ok(new ResponseSuccess(comentarioAlterado.Id));
        }

        /// <summary>
        /// Deletar um comentário
        /// </summary>
        /// <param name="id">ID do comentário</param>
        /// <returns>Retorna OK quando o comentário for deletado</returns>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletarComentario([FromRoute] Guid id)
        {
            var comentarioDeletado = await _comentariosService.DeletarComentarioAsync(id);
            if (comentarioDeletado != null)
                return Ok();

            return NotFound();
        }
    }
}
