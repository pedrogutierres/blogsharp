using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Components
{
    [ViewComponent(Name = "ComentariosViewComponent")]
    public class ComentariosViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ComentariosViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid postId)
        {
            ViewData["PostId"] = postId;

            return View(await _context.Comentarios.Include(c => c.Autor).Where(c => c.PostId == postId && !c.Excluido).OrderByDescending(p => p.DataHoraCriacao).ToListAsync());
        }
    }
}
