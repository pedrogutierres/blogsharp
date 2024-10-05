using Blog.Api.Helpers;

namespace Blog.Api.ViewModels.Posts
{
    public class PostResumidoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public string ConteudoNormalizado => CustomHtmlHelpers.RemoverTagsHtml(Conteudo);
        public string ConteudoResumidoNormalizado => ConteudoNormalizado.Length > 100 ? ConteudoNormalizado[0..100] : ConteudoNormalizado;
        public bool Excluido { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public Guid AutorId { get; set; }
        public string AutorNomeCompleto { get; set; }
    }
}
