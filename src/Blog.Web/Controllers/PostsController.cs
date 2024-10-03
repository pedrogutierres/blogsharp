using Blog.Data;
using Blog.Data.Models;
using Blog.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUser _user;

        public PostsController(ApplicationDbContext context, IUser user)
        {
            _context = context;
            _user = user;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Autor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var tempoDeLeituraMinutos = CalcularTempoLeituraEmMinutos(post.Conteudo.Length);
            ViewData["TempoDeLeitura"] = tempoDeLeituraMinutos > 1 ? $"{tempoDeLeituraMinutos} minutos de leitura" : "1 minuto de leitura";

            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> MeusPosts()
        {
            return View();
        }

        [Authorize]
        public IActionResult Create()
        {
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Conteudo")] Post post)
        {
            if (!ModelState.IsValid)
                return View(post);

            post.Id = Guid.NewGuid();
            post.AutorId = _user.UsuarioId().Value;

            _context.Add(post);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = post.Id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nome", post.AutorId);
            return View(post);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Titulo,Conteudo")] Post post)
        {
            if (id != post.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(post);

            var postOriginal = await _context.Posts.FindAsync(id);
            if (postOriginal == null)
                return NotFound();

            if (postOriginal.AutorId != _user?.UsuarioId())
                throw new UnauthorizedAccessException("Usuário não autorizado a editar o post pois não pertence ao mesmo.");

            postOriginal.Titulo = post.Titulo;
            postOriginal.Conteudo = post.Conteudo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(post.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (!(_user?.Autenticado() ?? false))
                throw new UnauthorizedAccessException("Usuário não autenticado");

            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                if (post.AutorId != _user.UsuarioId().Value)
                    throw new UnauthorizedAccessException("Usuário não autorizado a excluir o post pois não pertence ao mesmo.");

                post.Excluido = true;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ativar(Guid id)
        {
            if (!(_user?.Autenticado() ?? false))
                throw new UnauthorizedAccessException("Usuário não autenticado");

            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                if (post.AutorId != _user.UsuarioId().Value)
                    throw new UnauthorizedAccessException("Usuário não autorizado a ativar o post pois não pertence ao mesmo.");

                post.Excluido = false;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(Guid id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }


        private static long CalcularTempoLeituraEmMinutos(long numeroCaracteres)
        {
            long palavrasPorMinuto = 225; // Velocidade média de leitura de uma pessoa
            long caracteresPorPalavra = 5; // Estimativa de caracteres por palavra

            // Calcular o número de palavras
            long numeroPalavras = numeroCaracteres / caracteresPorPalavra;

            // Calcular o tempo em minutos
            return numeroPalavras / palavrasPorMinuto;
        }
    }
}
