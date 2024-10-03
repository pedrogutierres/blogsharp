using Blog.Web.Helpers;

namespace Blog.Web.ViewModels.Posts
{
    public class PostResumidoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public string ConteudoResumidoHtml => CustomHtmlHelpers.RemoverTagsHtml(Conteudo)[0..100];
        public bool Excluido { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public Guid AutorId { get; set; }
        public string AutorNomeCompleto { get; set; }
    }
}
