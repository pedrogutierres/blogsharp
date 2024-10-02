using Blog.Web.Helpers;

namespace Blog.Web.ViewModels.Posts
{
    public class PostResumidoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string ConteudoResumido { get; set; }
        public string ConteudoResumidoHtml => CustomHtmlHelpers.RemoverTagsHtml(ConteudoResumido);
        public DateTime DataHoraCriacao { get; set; }
        public Guid AutorId { get; set; }
        public string AutorNomeCompleto { get; set; }
    }
}
