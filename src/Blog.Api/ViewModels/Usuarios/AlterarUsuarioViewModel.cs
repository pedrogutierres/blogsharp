using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Usuarios
{
    /// <summary>
    /// Utilizado para alterar um usuário
    /// </summary>
    public class AlterarUsuarioViewModel
    {
        /// <summary>
        /// Nome do usuário (autor)
        /// </summary>
        [Required(ErrorMessage = "O nome deve ser informado.")]
        public string Nome { get; set; }

        /// <summary>
        /// Sobrenome do usuário (autor)
        /// </summary>
        [Required(ErrorMessage = "O sobrenome deve ser informado.")]
        public string Sobrenome { get; set; }

        /// <summary>
        /// Telefone do usuário (autor)
        /// </summary>
        [Phone(ErrorMessage = "O telefone está inválido.")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }
    }
}
