using Blog.Application.Helpers;
using Blog.Application.Services;
using Blog.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blog.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly PostService _postService;
        private readonly OpenAIOptions _options;

        public PostsController(PostService postService, IOptions<OpenAIOptions> options)
        {
            _postService = postService;
            _options = options.Value;
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
            ViewData["GerarImagemIA"] = !string.IsNullOrEmpty(_options?.ApiKey) ? "true" : "false";

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Conteudo")] Post post, IFormFile imagemUpload, IFormCollection form)
        {
            if (!ModelState.IsValid)
                return View(post);

            post.Id = Guid.NewGuid();

            if (imagemUpload != null)
            {
                using var memoryStream = new MemoryStream();
                await imagemUpload.CopyToAsync(memoryStream);
                post.Imagem = memoryStream.ToArray();
            }

            await _postService.PublicarPostAsync(post, form["gerarImagemIA"].ToString().Equals("on", StringComparison.CurrentCultureIgnoreCase));

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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Titulo,Conteudo")] Post post, IFormFile imagemUpload)
        {
            if (id != post.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(post);

            if (imagemUpload != null)
            {
                using var memoryStream = new MemoryStream();
                await imagemUpload.CopyToAsync(memoryStream);
                post.Imagem = memoryStream.ToArray();
            }

            var postAlterado = await _postService.EditarPostAsync(post, false);
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
