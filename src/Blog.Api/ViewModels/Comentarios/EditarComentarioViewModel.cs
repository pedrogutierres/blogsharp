using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Comentarios
{
    public class EditarComentarioViewModel
    {
        [Required(ErrorMessage = "O conteúdo do comentário deve ser informado.")]
        [StringLength(500, ErrorMessage = "O comentário deve conter no máximo {1} caracteres.")]
        public string Conteudo { get; set; }
    }
}
