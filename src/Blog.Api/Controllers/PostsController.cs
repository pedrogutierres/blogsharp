using AutoMapper;
using Blog.Api.ViewModels;
using Blog.Api.ViewModels.Posts;
using Blog.Application.Services;
using Blog.Application.ViewModels.Posts;
using Blog.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Blog.Api.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar os posts
    /// </summary>
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/posts")]
    public class PostsController : Controller
    {
        private readonly PostService _postService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="postService">Serviço para gerenciamento dos posts com a base de dados</param>
        /// <param name="mapper">Interface para facilitar a transferência de dados entre Models e DTO's</param>
        public PostsController(PostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        /// <summary>
        /// Obter todos os posts ativos
        /// </summary>
        /// <param name="meusPosts">Possibilita listar apenas os posts vinculados ao usuário logado</param>
        /// <returns>Retorna uma lista dos posts</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<PostQueryViewModel>), StatusCodes.Status200OK)]
        public Task<IEnumerable<PostQueryViewModel>> ObterPosts([FromQuery(Name = "meus-posts")] bool meusPosts = false)
        {
            return _postService.ObterPostsAsync(meusPosts);
        }

        /// <summary>
        /// Obter os detalhes completos de posts, como autor e comentários
        /// </summary>
        /// <param name="id">ID do post</param>
        /// <returns>Retorna os detalhes completo do post</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PostViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostViewModel>> ObterDetalhesDoPost([FromRoute] Guid id, [FromServices] ComentariosService comentariosService)
        {
            var post = await _postService.ObterPostPorIdAsync(id);
            if (post == null)
                return NotFound();

            var postMapper = _mapper.Map<Post, PostViewModel>(post);

            postMapper.Comentarios = _mapper.Map<IEnumerable<Comentario>, IEnumerable<PostComentarioViewModel>>(await comentariosService.ObterComentariosAsync(id));

            return postMapper;
        }

        /// <summary>
        /// Publicar um post
        /// </summary>
        /// <param name="viewModel">Dados do post</param>
        /// <returns>Retorna o ID do post publicado</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PublicarPost([FromBody] PublicarPostViewModel viewModel)
        {
            var post = _mapper.Map<PublicarPostViewModel, Post>(viewModel) ?? throw new ArgumentNullException();

            await _postService.PublicarPostAsync(post);

            return Ok(new ResponseSuccess(post.Id));
        }

        /// <summary>
        /// Editar um post
        /// </summary>
        /// <param name="id">ID do post</param>
        /// <param name="viewModel">Dados do post alterado</param>
        /// <returns>Retorna o ID do post alterado</returns>
        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditarPost([FromRoute] Guid id, [FromBody] EditarPostViewModel viewModel)
        {
            var post = _mapper.Map<EditarPostViewModel, Post>(viewModel) ?? throw new ArgumentNullException();
            post.Id = id;

            var postAlterado = await _postService.EditarPostAsync(post);
            if (postAlterado == null)
                return NotFound();

            return Ok(new ResponseSuccess(postAlterado.Id));
        }

        /// <summary>
        /// Deletar um post
        /// </summary>
        /// <param name="id">ID do post</param>
        /// <returns>Retorna OK quando o post for deletado</returns>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletarPost([FromRoute] Guid id)
        {
            if (await _postService.DeletarPostAsync(id))
                return Ok();

            return NotFound();
        }

        /// <summary>
        /// Ativar um posto que está desativado (excluído)
        /// </summary>
        /// <param name="id">ID do post</param>
        /// <returns>Retorna OK quando o post for ativado</returns>
        [HttpPatch("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtivarPost([FromRoute] Guid id)
        {
            if (await _postService.AtivarPostAsync(id))
                return Ok();

            return NotFound();
        }
    }
}
