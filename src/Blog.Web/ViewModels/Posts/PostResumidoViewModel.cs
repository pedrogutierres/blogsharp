using Blog.Web.Helpers;

namespace Blog.Web.ViewModels.Posts
{
    public class PostResumidoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public string ConteudoHtml => CustomHtmlHelpers.RemoverTagsHtml(Conteudo);
        public string ConteudoResumidoHtml => ConteudoHtml.Length > 100 ? ConteudoHtml[0..100] : ConteudoHtml;
        public bool Excluido { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public Guid AutorId { get; set; }
        public string AutorNomeCompleto { get; set; }
    }
}
