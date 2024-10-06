using System.ComponentModel.DataAnnotations;

namespace Blog.Api.ViewModels.Usuarios
{
    /// <summary>
    /// Utilizado para conectar um usuário
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// E-mail do usuário
        /// </summary>
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O {0} deve ser informado")]
        [EmailAddress(ErrorMessage = "O {0} está inválido")]
        public string Email { get; set; }

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "A {0} deve ser informada")]
        public string Senha { get; set; }
    }
}
