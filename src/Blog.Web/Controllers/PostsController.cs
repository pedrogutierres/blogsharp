using Blog.Application.Services;
using Blog.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly PostService _postService;

        public PostsController(PostService postService)
        {
            _postService = postService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var post = await _postService.ObterPostPorIdAsync(id.Value);
            if (post == null)
                return NotFound();

            var tempoDeLeituraMinutos = CalcularTempoLeituraEmMinutos(post.Conteudo.Length);
            ViewData["TempoDeLeitura"] = tempoDeLeituraMinutos > 1 ? $"{tempoDeLeituraMinutos} minutos de leitura" : "1 minuto de leitura";

            return View(post);
        }

        [Authorize]
        public IActionResult MeusPosts()
        {
            return View();
        }

        [Authorize]
        public IActionResult Create()
        {
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

            await _postService.PublicarPostAsync(post);

            return RedirectToAction(nameof(Details), new { id = post.Id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var post = await _postService.ObterPostPorIdAsync(id.Value);
            if (post == null)
                return NotFound();

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

            var postAlterado = await _postService.EditarPostAsync(post);
            if (postAlterado == null)
                return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _postService.DeletarPostAsync(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ativar(Guid id)
        {
            await _postService.AtivarPostAsync(id);

            return RedirectToAction(nameof(Index));
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
