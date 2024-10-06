using Blog.Business.Application.ViewModels.Posts;
using Blog.Data;
using Blog.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog.Business.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUser _user;

        public PostService(ApplicationDbContext context, IUser user)
        {
            _context = context;
            _user = user;
        }

        public async Task<IEnumerable<PostResumidoViewModel>> ObterPostsAsync(bool meusPosts = false)
        {
            var queryable = _context.Posts.Include(p => p.Autor).AsQueryable();

            if (meusPosts)
            {
                if (!(_user?.Autenticado() ?? false))
                    throw new UnauthorizedAccessException("Você deve estar logado para visualizar seus posts.");

                queryable = queryable.Where(p => p.AutorId == _user.UsuarioId().Value);
            }
            else
            {
                if (!(_user?.Autenticado() ?? false) || !_user.Administrador())
                    queryable = queryable.Where(p => !p.Excluido);
            }

            return await queryable.Select(p =>
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
            ).ToListAsync();
        }
    }
}
