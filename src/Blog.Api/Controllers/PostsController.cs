using Blog.Business.Application.ViewModels.Posts;
using Blog.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    [Route("api/posts")]
    public class PostsController
    {
        private readonly PostService _postService;

        public PostsController(PostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [AllowAnonymous]
        public Task<IEnumerable<PostResumidoViewModel>> ObterPosts([FromQuery(Name = "meus-posts")] bool meusPosts = false)
        {
           return _postService.ObterPostsAsync(meusPosts);
        }
    }
}
