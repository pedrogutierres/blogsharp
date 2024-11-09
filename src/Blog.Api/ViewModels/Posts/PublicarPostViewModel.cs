using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Posts
{
    /// <summary>
    /// Utilizado para publicar um post
    /// </summary>
    public class PublicarPostViewModel
    {
        /// <summary>
        /// Gerado automaticamente pelo sistema (não informar)
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Título do post
        /// </summary>
        [Required(ErrorMessage = "O título do post deve ser informado.")]
        [StringLength(200, ErrorMessage = "O título do post deve conter no máximo {1} caracteres")]
        public string Titulo { get; set; }

        /// <summary>
        /// Conteúdo do post
        /// </summary>
        [Required(ErrorMessage = "O conteúdo é obrigatório")]
        public string Conteudo { get; set; }

        /// <summary>
        /// Imagem do post em base64
        /// </summary>
        public string ImagemBase64 { get; set; }
    }
}
