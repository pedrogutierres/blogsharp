﻿using Blog.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Components
{
    public class ComentariosViewComponent : ViewComponent
    {
        private readonly ComentariosService _comentariosService;

        public ComentariosViewComponent(ComentariosService comentariosService)
        {
            _comentariosService = comentariosService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid postId)
        {
            ViewData["PostId"] = postId;

            return View(await _comentariosService.ObterComentariosAsync(postId));
        }
    }
}
