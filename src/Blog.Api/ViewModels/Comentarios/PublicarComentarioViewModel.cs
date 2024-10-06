using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Comentarios
{
    /// <summary>
    /// Utilizado para publicar um comentário
    /// </summary>
    public class PublicarComentarioViewModel
    {
        /// <summary>
        /// Gerado automaticamente pelo sistema (não informar)
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Conteúdo do comentário
        /// </summary>
        [Required(ErrorMessage = "O conteúdo do comentário deve ser informado.")]
        [StringLength(500, ErrorMessage = "O comentário deve conter no máximo {1} caracteres.")]
        public string Conteudo { get; set; }
    }
}
