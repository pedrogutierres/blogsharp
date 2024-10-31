using Blog.Application.Identity;
using Blog.Application.Queries;
using Blog.Data;
using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Application.Services
{
    public class PostService
    {
        private const string CacheKey_Posts = "Posts";
        private const string CacheKey_PostId = "Post";

        private readonly ApplicationDbContext _context;
        private readonly IUser _user;
        private readonly IMemoryCache _cache;

        public PostService(ApplicationDbContext context, IUser user, IMemoryCache cache)
        {
            _context = context;
            _user = user;
            _cache = cache;
        }

        public async Task<IEnumerable<PostQueryModel>> ObterPostsAsync(bool meusPosts = false)
        {
            // Irá ter cache apenas para os Posts em geral, caso for meus posts ou usuário administrador, deverá mostrar sempre atualizado sem cache
            var cacheKey = (_user?.Administrador() ?? false) || meusPosts ? null : CacheKey_Posts;

            if (cacheKey != null)
            {
                return await _cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    return await ObterPostsQueryAsync(meusPosts);
                });
            }
            else
                return await ObterPostsQueryAsync(meusPosts);
        }
        private async Task<IEnumerable<PostQueryModel>> ObterPostsQueryAsync(bool meusPosts = false)
        {
            var queryable = _context.Posts.Include(p => p.Autor).OrderByDescending(p => p.DataHoraCriacao).AsQueryable();

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
                new PostQueryModel
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

        public Task<Post> ObterPostPorIdAsync(Guid id) => _cache.GetOrCreateAsync($"{CacheKey_PostId}-{id}", entry => _context.Posts.Include(p => p.Autor).FirstOrDefaultAsync(m => m.Id == id));

        public async Task<bool> PublicarPostAsync(Post post)
        {
            post.AutorId = _user.UsuarioId().Value;

            _context.Add(post);

            _cache.Remove(CacheKey_Posts);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Post> EditarPostAsync(Post post)
        {
            var postOriginal = await _context.Posts.FindAsync(post.Id);
            if (postOriginal == null)
                return null;

            if (!UsuarioAutorizado(postOriginal.AutorId))
                throw new UnauthorizedAccessException("Usuário não autorizado a editar o post pois não pertence ao mesmo.");

            postOriginal.Titulo = post.Titulo;
            postOriginal.Conteudo = post.Conteudo;

            try
            {
                await _context.SaveChangesAsync();

                LimparCachePostId(postOriginal.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await PostExists(post.Id))
                    throw;

                return null;
            }

            return postOriginal;
        }

        public async Task<bool> DeletarPostAsync(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return false;

            if (!UsuarioAutorizado(post.AutorId))
                throw new UnauthorizedAccessException("Usuário não autorizado a excluir o post pois não pertence ao mesmo.");

            post.Excluido = true;

            await _context.SaveChangesAsync();
            
            LimparCachePostId(id);

            return true;
        }

        public async Task<bool> AtivarPostAsync(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return false;

            if (!UsuarioAutorizado(post.AutorId))
                throw new UnauthorizedAccessException("Usuário não autorizado a excluir o post pois não pertence ao mesmo.");

            post.Excluido = false;

            await _context.SaveChangesAsync();

            LimparCachePostId(id);

            return true;
        }

        private void LimparCachePostId(Guid id)
        {
            _cache.Remove(CacheKey_Posts);
            _cache.Remove($"{CacheKey_PostId}-{id}");
        }

        private async Task<bool> PostExists(Guid id) => await _context.Posts.AnyAsync(e => e.Id == id);

        private bool UsuarioAutorizado(Guid id) => _user.Administrador() || id == _user.UsuarioId().Value;
    }
}
