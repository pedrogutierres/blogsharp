using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Comentarios
{
    /// <summary>
    /// Utilizado para editar um comentário
    /// </summary>
    public class EditarComentarioViewModel
    {
        /// <summary>
        /// Conteúdo do comentário a ser alterado
        /// </summary>
        [Required(ErrorMessage = "O conteúdo do comentário deve ser informado.")]
        [StringLength(500, ErrorMessage = "O comentário deve conter no máximo {1} caracteres.")]
        public string Conteudo { get; set; }
    }
}
