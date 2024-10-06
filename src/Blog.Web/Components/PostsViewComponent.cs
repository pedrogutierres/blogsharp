using Blog.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Components
{
    public class PostsViewComponent : ViewComponent
    {
        private readonly PostService _postService;

        public PostsViewComponent(PostService postService)
        {
            _postService = postService;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool meusPosts = false)
        {
            ViewData["meus-posts"] = meusPosts.ToString();

            return View(await _postService.ObterPostsAsync(meusPosts));
        }
    }
}
