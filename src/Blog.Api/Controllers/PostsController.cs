using AutoMapper;
using Blog.Api.ViewModels;
using Blog.Api.ViewModels.Posts;
using Blog.Business.Application.ViewModels.Posts;
using Blog.Business.Services;
using Blog.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Blog.Api.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/posts")]
    public class PostsController : Controller
    {
        private readonly PostService _postService;
        private readonly IMapper _mapper;

        public PostsController(PostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<PostResumidoViewModel>), StatusCodes.Status200OK)]
        public Task<IEnumerable<PostResumidoViewModel>> ObterPosts([FromQuery(Name = "meus-posts")] bool meusPosts = false)
        {
            return _postService.ObterPostsAsync(meusPosts);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PostViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostViewModel>> ObterDetalhesDoPost([FromRoute] Guid id)
        {
            var post = await _postService.ObterPostPorIdAsync(id);
            if (post == null)
                return NotFound();

            return _mapper.Map<Post, PostViewModel>(post);
        }

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
