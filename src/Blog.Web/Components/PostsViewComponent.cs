using Blog.Data;
using Blog.Web.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Components
{
    public class PostsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public PostsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Autor).Select(p =>
                new PostResumidoViewModel
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    ConteudoResumido = p.Conteudo.Substring(0, 100),
                    DataHoraCriacao = p.DataHoraCriacao,
                    AutorId = p.AutorId,
                    AutorNomeCompleto = $"{p.Autor.Nome} {p.Autor.Sobrenome}"
                }
            );

            return View(await applicationDbContext.ToListAsync());
        }
    }
}
