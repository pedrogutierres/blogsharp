using Blog.Data;
using Blog.Identity.Interfaces;
using Blog.Web.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Components
{
    public class PostsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IUser _user;

        public PostsViewComponent(ApplicationDbContext context, IUser user)
        {
            _context = context;
            _user = user;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool meusPosts = false)
        {
            ViewData["meus-posts"] = meusPosts.ToString();

            var queryable = _context.Posts.Include(p => p.Autor).AsQueryable();

            if (meusPosts)
            {
                if (!(_user?.Autenticado() ?? false))
                    throw new UnauthorizedAccessException("Você deve estar logado para visualizar seus posts.");

                queryable = queryable.Where(p => p.AutorId == _user.UsuarioId().Value);
            }
            else
                queryable = queryable.Where(p => !p.Excluido);

            return View(await queryable.Select(p =>
                new PostResumidoViewModel
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    Conteudo = p.Conteudo,
                    Excluido = p.Excluido,
                    DataHoraCriacao = p.DataHoraCriacao,
                    AutorId = p.AutorId,
                    AutorNomeCompleto = $"{p.Autor.Nome} {p.Autor.Sobrenome}"
                }
            ).ToListAsync());
        }
    }
}
