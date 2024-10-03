using Blog.Data;
using Blog.Data.Models;
using Blog.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Blog.Web.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUser _user;

        public ComentariosController(ApplicationDbContext context, IUser user)
        {
            _context = context;
            _user = user;
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
            comentario.AutorId = _user.UsuarioId().Value;

            _context.Add(comentario);

            await _context.SaveChangesAsync();

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

            var comentarioOriginal = await _context.Comentarios.FindAsync(id);
            if (comentarioOriginal == null)
                return NotFound();

            if (comentarioOriginal.AutorId != _user?.UsuarioId())
                throw new UnauthorizedAccessException("Usuário não autorizado a editar o comentário que não é dele.");

            comentarioOriginal.Conteudo = comentario.Conteudo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComentarioExists(comentario.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Details), "Posts", new { id = comentarioOriginal.PostId });
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (!(_user?.Autenticado() ?? false))
                throw new UnauthorizedAccessException("Usuário não autenticado");

            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario != null)
            {
                if (comentario.AutorId != _user.UsuarioId().Value)
                    throw new UnauthorizedAccessException("Usuário não autorizado a excluir o comentário que não é dele.");

                comentario.Excluido = true;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), "Posts", new { id = comentario.PostId });
        }

        private bool ComentarioExists(Guid id)
        {
            return _context.Comentarios.Any(e => e.Id == id);
        }
    }
}
