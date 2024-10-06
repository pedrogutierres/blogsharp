using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Posts
{
    /// <summary>
    /// Utilizado para editar um post
    /// </summary>
    public class EditarPostViewModel
    {
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
    }
}
