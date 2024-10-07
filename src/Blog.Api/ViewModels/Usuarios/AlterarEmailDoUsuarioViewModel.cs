using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Usuarios
{
    /// <summary>
    /// Utilizado para alterar o e-mail de um usuário
    /// </summary>
    public class AlterarEmailDoUsuarioViewModel
    {
        /// <summary>
        /// Novo E-mail do usuário
        /// </summary>
        [Display(Name = "Novo E-mail")]
        [Required(ErrorMessage = "O {0} deve ser informado")]
        [EmailAddress(ErrorMessage = "O {0} está inválido")]
        public string NovoEmail { get; set; }
    }
}
