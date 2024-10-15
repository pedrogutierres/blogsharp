using Blog.Application.Identity;
using Blog.Data;
using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Application.Services
{
    public class ComentariosService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUser _user;

        public ComentariosService(ApplicationDbContext context, IUser user)
        {
            _context = context;
            _user = user;
        }

        public async Task<IEnumerable<Comentario>> ObterComentariosAsync(Guid postId)
        {
            return await _context.Comentarios.Include(c => c.Autor).Where(c => c.PostId == postId && !c.Excluido).OrderByDescending(p => p.DataHoraCriacao).ToListAsync();
        }

        public async Task<bool> PublicarComentarioAsync(Comentario comentario)
        {
            comentario.AutorId = _user.UsuarioId().Value;

            _context.Add(comentario);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Comentario> EditarComentarioAsync(Comentario comentario)
        {
            var comentarioOriginal = await _context.Comentarios.FindAsync(comentario.Id);
            if (comentarioOriginal == null)
                return null;

            if (!_user.Administrador() && comentarioOriginal.AutorId != _user.UsuarioId().Value)
                throw new UnauthorizedAccessException("Usuário não autorizado a editar o comentário que não é dele.");

            comentarioOriginal.Conteudo = comentario.Conteudo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await ComentarioExists(comentario.Id))
                    throw;

                return null;
            }

            return comentarioOriginal;
        }

        public async Task<Comentario> DeletarComentarioAsync(Guid id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
                return null;

            if (!_user.Administrador() && comentario.AutorId != _user.UsuarioId().Value)
                throw new UnauthorizedAccessException("Usuário não autorizado a excluir o comentário que não é dele.");

            comentario.Excluido = true;
            comentario.DataHoraExclusao = DateTime.Now;

            await _context.SaveChangesAsync();

            return comentario;
        }

        private async Task<bool> ComentarioExists(Guid id) => await _context.Comentarios.AnyAsync(e => e.Id == id);
    }
}
