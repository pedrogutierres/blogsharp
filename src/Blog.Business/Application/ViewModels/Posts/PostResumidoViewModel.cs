using Blog.Business.Helpers;

namespace Blog.Business.Application.ViewModels.Posts
{
    /// <summary>
    /// Dados resumidos de um post
    /// </summary>
    public class PostResumidoViewModel
    {
        /// <summary>
        /// Identificador único
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Título do post
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Conteúdo do post (podendo ser em HTML)
        /// </summary>
        public string Conteudo { get; set; }

        /// <summary>
        /// Conteúdo do post sem tags HTML
        /// </summary>
        public string ConteudoNormalizado => CustomHtmlHelpers.RemoverTagsHtml(Conteudo);

        /// <summary>
        /// Conteúdo do post sem tags HTml e com no máximo 100 caracteres
        /// </summary>
        public string ConteudoResumidoNormalizado => ConteudoNormalizado.Length > 100 ? ConteudoNormalizado[0..100] : ConteudoNormalizado;

        /// <summary>
        /// Informa se o posto foi excluído
        /// </summary>
        public bool Excluido { get; set; }

        /// <summary>
        /// Data e hora da publicação do post
        /// </summary>
        public DateTime DataHoraCriacao { get; set; }

        /// <summary>
        /// Identificador único do autor do post
        /// </summary>
        public Guid AutorId { get; set; }

        /// <summary>
        /// Nome completo do autor do post
        /// </summary>
        public string AutorNomeCompleto { get; set; }
    }
}
