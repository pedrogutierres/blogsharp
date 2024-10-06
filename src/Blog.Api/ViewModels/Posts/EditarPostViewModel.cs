using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Posts
{
    public class EditarPostViewModel
    {
        [Required(ErrorMessage = "O título do post deve ser informado.")]
        [StringLength(200, ErrorMessage = "O título do post deve conter no máximo {1} caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O conteúdo é obrigatório")]
        public string Conteudo { get; set; }
    }
}
