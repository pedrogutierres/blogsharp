using Blog.Application.Services;
using Blog.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Blog.Web.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly ComentariosService _comentariosService;

        public ComentariosController(ComentariosService comentariosService)
        {
            _comentariosService = comentariosService;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Conteudo,PostId")] Comentario comentario)
        {
            if (!ModelState.IsValid)
            {
                TempData["ComentariosModelStateErrors"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return RedirectToAction(nameof(Details), "Posts", new { id = comentario.PostId });
            }

            comentario.Id = Guid.NewGuid();

            await _comentariosService.PublicarComentarioAsync(comentario);

            return RedirectToAction(nameof(Details), "Posts", new { id = comentario.PostId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Conteudo,PostId")] Comentario comentario)
        {
            if (id != comentario.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(comentario);

            var comentarioAlterado = await _comentariosService.EditarComentarioAsync(comentario);
            if (comentarioAlterado == null)
                return NotFound();

            return RedirectToAction(nameof(Details), "Posts", new { id = comentarioAlterado.PostId });
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var comentario = await _comentariosService.DeletarComentarioAsync(id);
            if (comentario == null)
                return NotFound();

            return RedirectToAction(nameof(Details), "Posts", new { id = comentario.PostId });
        }
    }
}
